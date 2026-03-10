using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Data.Experimental;
using Project.Scripts.Runtime.Events;

namespace Project.Scripts.Data.Audio
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