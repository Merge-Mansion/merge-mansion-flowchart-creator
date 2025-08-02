using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeMansion
{
    public class AreaCollection
    {
        public string CreatedAt { get; set; }
        public List<AreaData> Data { get; set; }
    }

    public class AreaData
    {
        public string Name { get; set; }
        public string TaskDependencies { get; set; }
        public string AreaId { get; set; }
        public string TitleLocalizationId { get; set; }
        public List<object> TeaseRequirements { get; set; }
        public List<object> UnlockRequirements { get; set; }
        public List<Reward> Rewards { get; set; }
        public List<HotspotRef> HotspotsRefs { get; set; }
    }

    public class Reward
    {
        public RewardItem RewardItem { get; set; } 
    }

    public class RewardItem
    {
        public string ItemRef { get; set; }
        public int Amount { get; set; }
        public bool FromSupport { get; set; }
        public string MergeBoardId { get; set; }
        public bool ForceOnTopOfPocket { get; set; }
        public string Source { get; set; }
    }

    public class HotspotRef
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string AreaRef { get; set; }
        public string MergeBoardId { get; set; }
        public List<RequirementList> RequirementsList { get; set; }
        public List<string> UnlockingParentRefs { get; set; }
        public List<Reward> Rewards { get; set; }
        public List<object> CompletionActions { get; set; }
        public List<object> FinalizationActions { get; set; }
        public List<object> AppearActions { get; set; }
        public MapSpotRef MapSpotRef { get; set; }
        public object TaskGroupId { get; set; }
        public List<object> UnlockRequirementsList { get; set; }
        public bool IsIndependentTask { get; set; }
        public int AppearActionMax { get; set; }
        public int CompleteActionMax { get; set; }
        public object CompleteFocusHotspotRef { get; set; }
        public List<object> AppearMapCharactersEventsRefs { get; set; }
        public List<object> CompleteMapCharactersEventsRefs { get; set; }
        public int BonusTimerDuration { get; set; }
        public List<object> BonusRewards { get; set; }
        public object CustomHotspotTableId { get; set; }
        public int SoloMilestoneHotspotValue { get; set; }
        public object MultistepGroupId { get; set; }
        public int BoultonLeaguePoints { get; set; }
        public bool DelayDebrisAnimation { get; set; }
        public int Difficulty { get; set; }
        public List<object> DifficultyRewards { get; set; }
    }

    public class RequirementList
    {
        public List<ItemAcquired> ItemAcquired { get; set; }
    }

    public class ItemAcquired
    {
        public string ItemRef { get; set; }
        public int Requirement { get; set; }
    }

    public class MapSpotRef
    {
        public string ConfigKey { get; set; }
        public string TitleId { get; set; }
    }

}
