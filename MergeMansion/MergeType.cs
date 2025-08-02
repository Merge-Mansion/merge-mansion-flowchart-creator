using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static MergeMansion.MergeType;

namespace MergeMansion
{
    public class MergeType
    {
        public class Root
        {
            public DateTime CreatedAt { get; set; }
            public List<DrawerData> Data { get; set; }
        }

        public class DrawerData
        {
            public string Name { get; set; }
            public string ConfigKey { get; set; }
            public List<PrimaryChain> PrimaryChain { get; set; }
            public List<object> FallbackChain { get; set; }
            public CodexCategory CodexCategory { get; set; }
            public DiscoveryRewardRef DiscoveryRewardRef { get; set; }
            public int UnsellableUntilPlayerLevel { get; set; }
            public int ShowSellConfirmationUntilPlayerLevel { get; set; }
        }

        public class PrimaryChain
        {
            public Item Item { get; set; }
            public int Count { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int SellCoins { get; set; }
            public int RequiredItemValue { get; set; }
            public int RewardItemValue { get; set; }
            public int ConfigKey { get; set; }
            public string PoolTag { get; set; }
            public string SkinName { get; set; }
            public int LevelNumber { get; set; }
            public bool Movable { get; set; }
            public double CostInDiamonds { get; set; }
            public double AnchorPriceGems { get; set; }
            public double AnchorPriceCoins { get; set; }
            public double TimeSkipPriceGems { get; set; }
            public double UnlockOnBoardPriceGems { get; set; }
            public int ExperienceValue { get; set; }
            public MergeFeatures MergeFeatures { get; set; }
            public BubbleFeatures BubbleFeatures { get; set; }
            public List<string> Tags { get; set; }
            public string MergeChainRef { get; set; }
            public List<object> ConfirmableMergeResults { get; set; }
            public List<object> OnDiscoveredActions { get; set; }
            public bool ShowTutorialFingerOnDiscovery { get; set; }
            public List<string> AnalyticsMetaData { get; set; }
            public List<object> CombineInfoWithItem { get; set; }
            public string Rarity { get; set; }
            public bool Unsellable { get; set; }
            public string ItemType { get; set; }
            public List<object> Rewards { get; set; }
            public List<object> UnlockRequirements { get; set; }
            public List<object> SpawnEffects { get; set; }
            public object CustomItemInfoPopupId { get; set; }
            public bool ShowCustomItemInfoPopupOnDiscovery { get; set; }
            public int SinkPoints { get; set; }
            public List<object> OverrideProductionSource { get; set; }
            public ActivationFeatures ActivationFeatures { get; set; }
        }

        public class MergeFeatures
        {
            public Mechanic Mechanic { get; set; }
            public AdditionalSpawnProducer AdditionalSpawnProducer { get; set; }
            public bool Mergeable { get; set; }
            public bool RequiresXpState { get; set; }
        }

        public class Mechanic
        {
            public ResultProducer ResultProducer { get; set; }
            public string ResultVisibility { get; set; }
            public bool ResetTimers { get; set; }
            public string StorageAction { get; set; }
        }

        public class ResultProducer
        {
            public string Constant { get; set; }
        }

        public class AdditionalSpawnProducer
        {
            public string Constant { get; set; }
        }

        public class BubbleFeatures
        {
            public int BubbleDuration { get; set; }
            public string OpenCurrency { get; set; }
            public int OpenQuantity { get; set; }
            public int SpawnOdds { get; set; }
        }

        public class ActivationFeatures
        {
            public ActivationSpawn ActivationSpawn { get; set; }
            public Placement Placement { get; set; }
            public ActivationCycle ActivationCycle { get; set; }
            public int? StorageMax { get; set; }
            public string SpawnVisibility { get; set; }
            public bool? Activable { get; set; }
            public bool? StartsFull { get; set; }
            public bool? DecayAfterLastCycleAndActivation { get; set; }
            public bool? ShowTapTextOnDiscovery { get; set; }
            public bool? AllowCooldownRemover { get; set; }
            public bool? AllowEnergyMode { get; set; }
        }

        public class ActivationSpawn
        {
            public string Marker { get; set; }
            public BaseProducer BaseProducer { get; set; }
            public ControlledRandom ControlledRandom { get; set; }
            public string Constant { get; set; }
        }
        public class BaseProducer
        {
            public ControlledRandom ControlledRandom { get; set; }
        }

        public class ControlledRandom
        {
            public string RollType { get; set; }
            public string ItemType { get; set; }
            public Dictionary<string, double> Odds { get; set; }
        }

        public class Placement
        {
        }

        public class ActivationCycle
        {
            public int ActivationDelay { get; set; }
            public int FirstCycleStartDelay { get; set; }
            public int DelayBetweenCycles { get; set; }
            public int HowManyAreGeneratedInCycle { get; set; }
            public int ActivationAmountInCycle { get; set; }
            public int HowManyCycles { get; set; }
        }

        public class CodexCategory
        {
            public string ConfigKey { get; set; }
            public string IconItem { get; set; }
        }

        public class DiscoveryRewardRef
        {
            public string ConfigKey { get; set; }
            public DiscoveryCompletionReward DiscoveryCompletionReward { get; set; }
            public List<object> DiscoveryRewards { get; set; }
        }

        public class DiscoveryCompletionReward
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

        public class ActivationSpawnConverter : JsonConverter<ActivationSpawn>
        {
            public override ActivationSpawn Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var activationSpawn = new ActivationSpawn();

                if (reader.TokenType == JsonTokenType.String)
                {
                    activationSpawn.Constant = reader.GetString();
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var jsonDocument = JsonDocument.ParseValue(ref reader);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("Marker", out var markerElement))
                    {
                        activationSpawn.Marker = markerElement.GetString();
                    }

                    if (root.TryGetProperty("BaseProducer", out var baseProducerElement))
                    {
                        activationSpawn.BaseProducer = JsonSerializer.Deserialize<BaseProducer>(baseProducerElement.GetRawText(), options);
                    }

                    if (root.TryGetProperty("ControlledRandom", out var controlledRandomElement))
                    {
                        activationSpawn.ControlledRandom = JsonSerializer.Deserialize<ControlledRandom>(controlledRandomElement.GetRawText(), options);
                    }
                }
                else
                {
                    throw new JsonException("Invalid ActivationSpawn structure.");
                }

                return activationSpawn;
            }

            public override void Write(Utf8JsonWriter writer, ActivationSpawn value, JsonSerializerOptions options)
            {
                if (value.Constant != null)
                {
                    writer.WriteStringValue(value.Constant);
                }
                else
                {
                    writer.WriteStartObject();
                    if (value.Marker != null)
                    {
                        writer.WriteString("Marker", value.Marker);
                    }
                    if (value.BaseProducer != null)
                    {
                        writer.WritePropertyName("BaseProducer");
                        JsonSerializer.Serialize(writer, value.BaseProducer, options);
                    }
                    if (value.ControlledRandom != null)
                    {
                        writer.WritePropertyName("ControlledRandom");
                        JsonSerializer.Serialize(writer, value.ControlledRandom, options);
                    }
                    writer.WriteEndObject();
                }
            }
        }

        public class ResultProducerConverter : JsonConverter<ResultProducer>
        {
            public override ResultProducer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var resultProducer = new ResultProducer();

                if (reader.TokenType == JsonTokenType.String)
                {
                    resultProducer.Constant = reader.GetString();
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var jsonDocument = JsonDocument.ParseValue(ref reader);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("Constant", out var constantElement))
                    {
                        resultProducer.Constant = constantElement.GetString();
                    }
                }
                else
                {
                    throw new JsonException("Invalid ResultProducer structure.");
                }

                return resultProducer;
            }

            public override void Write(Utf8JsonWriter writer, ResultProducer value, JsonSerializerOptions options)
            {
                if (value.Constant != null)
                {
                    writer.WriteStringValue(value.Constant);
                }
                else
                {
                    writer.WriteStartObject();
                    if (value.Constant != null)
                    {
                        writer.WriteString("Constant", value.Constant);
                    }
                    writer.WriteEndObject();
                }
            }
        }

    }
}
