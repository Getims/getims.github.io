using Project.Scripts.Core.Events;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Data.Values;

namespace Project.Scripts.Core.Sounds.Data
{
    public interface ISoundDataService
    {
        DataValue<SoundData, bool> IsSoundOn { get; }
        DataValue<SoundData, bool> IsMusicOn { get; }
    }

    public class SoundDataService : ADataService<SoundData>, ISoundDataService
    {
        public DataValue<SoundData, bool> IsSoundOn { get; private set; }
        public DataValue<SoundData, bool> IsMusicOn { get; private set; }

        public SoundDataService(IDatabase database, GlobalEventProvider globalEventProvider) : base(database)
        {
            IsSoundOn = CreateValue(
                data => data.IsSoundOn,
                (data, value) => data.IsSoundOn = value);
            IsMusicOn = CreateValue(
                data => data.IsMusicOn,
                (data, value) => data.IsMusicOn = value);
        }
    }
}