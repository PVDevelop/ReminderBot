using System;
using System.IO;
using Newtonsoft.Json;
using PVDevelop.ReminderBot.Microservice.Port.Persistance;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Persistance
{
    public class FileUpdatesPositionRepository : IUpdatesPositionRepository
    {
        private readonly string _filePath;

        public FileUpdatesPositionRepository(string dataDirectory)
        {
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));

            _filePath = Path.Combine(dataDirectory, "updates_position.json");
        }

        public long GetPosition()
        {
            if (!File.Exists(_filePath))
            {
                return 0;
            }

            var fileText = File.ReadAllText(_filePath);

            var updatePositionDto = JsonConvert.DeserializeObject<UpdatePositionDto>(fileText);

            return updatePositionDto.Position;
        }

        public void SetPosition(long position)
        {
            var positionDto = new UpdatePositionDto(position);

            var fileText = JsonConvert.SerializeObject(positionDto);

            File.WriteAllText(_filePath, fileText);
        }
    }
}
