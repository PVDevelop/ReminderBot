using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Exceptions;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Bus;
using PVDevelop.ReminderBot.Microservice.Port.Infrastructure;
using PVDevelop.ReminderBot.Microservice.Port.Persistance;
using PVDevelop.ReminderBot.Microservice.Port.Telegram;
using PVDevelop.ReminderBot.Microservice.Request;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure
{
    internal sealed class UpdatesPuller : IRunable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<UpdatesPuller>();

        private readonly ITelegramClient _telegramClient;

        private readonly IUpdatesPositionRepository _updatesPositionRepository;

        private readonly IMessageBus _messageBus;

        private bool _firstPositionsRequest;

        public UpdatesPuller(
            ITelegramClient telegramClient,
            IUpdatesPositionRepository updatesPositionRepository,
            IMessageBus messageBus)
        {
            if (telegramClient == null) throw new ArgumentNullException(nameof(telegramClient));
            if (updatesPositionRepository == null) throw new ArgumentNullException(nameof(updatesPositionRepository));
            if (messageBus == null) throw new ArgumentNullException(nameof(messageBus));

            _firstPositionsRequest = true;

            _telegramClient = telegramClient;
            _updatesPositionRepository = updatesPositionRepository;
            _messageBus = messageBus;
        }

        public Task Run(CancellationToken token)
        {
            return Task.Run(() =>
            {
                Logger.Info("Starting pulling updates.");

                DoPulling(token);

                Logger.Info("Pulling updates finished.");
            }, token);
        }

        private void DoPulling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    ProcessUpdates(token);
                }
                catch (MicroserviceException ex)
                {
                    Logger.Warning(ex, "Exception while processing updates.");
                }
                catch (OperationCanceledException)
                {
                    Logger.Info("Pulling has been stopped.");
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex, "Unknown exception. Pulling will be interrupted.");
                    throw;
                }
            }
        }

        private void ProcessUpdates(CancellationToken token)
        {
            var offset = _updatesPositionRepository.GetPosition();

            if (_firstPositionsRequest)
            {
                Logger.Info($"Starting with offset: {offset}.");
                _firstPositionsRequest = false;
            }

            var responseDto = GetUpdates(offset, token);

            if (token.IsCancellationRequested)
            {
                Logger.Debug("Finishing process updates - cancellation requested.");
                return;
            }

            if (responseDto.IsOk)
            {
                SendUpdates(responseDto.Result);

                SaveOffset(responseDto.Result);
            }
            else
            {
                Logger.Warning($"Failed to get updates: {responseDto.Description}");
            }
        }

        private void SaveOffset(IReadOnlyCollection<UpdateDto> updates)
        {
            if (!updates.Any())
            {
                return;
            }

            var maxUpdateId = updates.Max(update => update.UpdateId) + 1;

            _updatesPositionRepository.SetPosition(maxUpdateId);
        }

        private ResponseDto<UpdateDto[]> GetUpdates(long offset, CancellationToken token)
        {
            const int requestLimit = 100;
            var getUpdatesDto = new GetUpdatesDto(offset, requestLimit);
            return _telegramClient.GetUpdates(getUpdatesDto, token);
        }

        private void SendUpdates(IReadOnlyCollection<UpdateDto> updates)
        {
            if (!updates.Any())
            {
                return;
            }

            var command = new ProcessUpdateMessagesCommand(updates);
            var message = new Message(command);

            _messageBus.SendMessage(message);
        }
    }
}
