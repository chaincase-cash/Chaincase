using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WalletWasabi.Bases;
using WalletWasabi.Exceptions;
using WalletWasabi.Helpers;
using WalletWasabi.Interfaces;
using WalletWasabi.JsonConverters;
using WalletWasabi.Logging;

namespace WalletWasabi.Backend
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Config : ConfigBase
	{
		public Config() : base()
		{
		}

		public Config(string filePath) : base(filePath)
		{
		}

		public Config(
			Network network,
			string bitcoinRpcConnectionString,
			EndPoint mainNetBitcoinP2pEndPoint,
			EndPoint testNetBitcoinP2pEndPoint,
			EndPoint regTestBitcoinP2pEndPoint,
			EndPoint mainNetBitcoinCoreRpcEndPoint,
			EndPoint testNetBitcoinCoreRpcEndPoint,
			EndPoint regTestBitcoinCoreRpcEndPoint,
			string databaseConnectionStringName)
			: base()
		{
			Network = Guard.NotNull(nameof(network), network);
			BitcoinRpcConnectionString = Guard.NotNullOrEmptyOrWhitespace(nameof(bitcoinRpcConnectionString), bitcoinRpcConnectionString);

			MainNetBitcoinP2pEndPoint = Guard.NotNull(nameof(mainNetBitcoinP2pEndPoint), mainNetBitcoinP2pEndPoint);
			TestNetBitcoinP2pEndPoint = Guard.NotNull(nameof(testNetBitcoinP2pEndPoint), testNetBitcoinP2pEndPoint);
			RegTestBitcoinP2pEndPoint = Guard.NotNull(nameof(regTestBitcoinP2pEndPoint), regTestBitcoinP2pEndPoint);

			MainNetBitcoinCoreRpcEndPoint = Guard.NotNull(nameof(mainNetBitcoinCoreRpcEndPoint), mainNetBitcoinCoreRpcEndPoint);
			TestNetBitcoinCoreRpcEndPoint = Guard.NotNull(nameof(testNetBitcoinCoreRpcEndPoint), testNetBitcoinCoreRpcEndPoint);
			RegTestBitcoinCoreRpcEndPoint = Guard.NotNull(nameof(regTestBitcoinCoreRpcEndPoint), regTestBitcoinCoreRpcEndPoint);
			DatabaseConnectionStringName = Guard.NotNull(nameof(databaseConnectionStringName), databaseConnectionStringName);
		}

		[JsonProperty(PropertyName = "Network")]
		[JsonConverter(typeof(NetworkJsonConverter))]
		public Network Network { get; private set; } = Network.Main;

		[DefaultValue("user:password")]
		[JsonProperty(PropertyName = "BitcoinRpcConnectionString", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string BitcoinRpcConnectionString { get; private set; }

		[JsonProperty(PropertyName = "MainNetBitcoinP2pEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultMainNetBitcoinP2pPort)]
		public EndPoint MainNetBitcoinP2pEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultMainNetBitcoinP2pPort);

		[JsonProperty(PropertyName = "TestNetBitcoinP2pEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultTestNetBitcoinP2pPort)]
		public EndPoint TestNetBitcoinP2pEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultTestNetBitcoinP2pPort);

		[JsonProperty(PropertyName = "RegTestBitcoinP2pEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultRegTestBitcoinP2pPort)]
		public EndPoint RegTestBitcoinP2pEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultRegTestBitcoinP2pPort);

		[JsonProperty(PropertyName = "MainNetBitcoinCoreRpcEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultMainNetBitcoinCoreRpcPort)]
		public EndPoint MainNetBitcoinCoreRpcEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultMainNetBitcoinCoreRpcPort);

		[JsonProperty(PropertyName = "TestNetBitcoinCoreRpcEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultTestNetBitcoinCoreRpcPort)]
		public EndPoint TestNetBitcoinCoreRpcEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultTestNetBitcoinCoreRpcPort);

		[JsonProperty(PropertyName = "RegTestBitcoinCoreRpcEndPoint")]
		[JsonConverter(typeof(EndPointJsonConverter), Constants.DefaultRegTestBitcoinCoreRpcPort)]
		public EndPoint RegTestBitcoinCoreRpcEndPoint { get; internal set; } = new IPEndPoint(IPAddress.Loopback, Constants.DefaultRegTestBitcoinCoreRpcPort);

		[DefaultValue("User ID=postgres;Host=127.0.0.1;Port=65466;Database=wasabibackend;")]
		[JsonProperty(PropertyName = "DatabaseConnectionStringName", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string DatabaseConnectionStringName { get; internal set; }

		[DefaultValue(0)]
		[JsonProperty(PropertyName = "HashCashDifficulty", DefaultValueHandling = DefaultValueHandling.Populate)]
		public int HashCashDifficulty { get; internal set; }

		[DefaultValue("APNAuth.p8")]
		[JsonProperty(PropertyName = "APNsAuthKeyFile", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string APNsAuthKeyFile { get; internal set; }
		[DefaultValue("A4L3728R8LJ")]
		[JsonProperty(PropertyName = "APNAuthKeyId", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string APNAuthKeyId { get; internal set; }

		[DefaultValue("9Z72DXKVXK")]
		[JsonProperty(PropertyName = "APNTeamId", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string APNTeamId { get; internal set; }
		[DefaultValue("cash.chaincase.testnet")]
		[JsonProperty(PropertyName = "APNBundleId", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string APNBundleId { get; internal set; }


		public string GetAPNAuthKeyFile()
		{
			return Path.Combine(new FileInfo(FilePath).Directory!.FullName, APNsAuthKeyFile);
		}

		public EndPoint GetBitcoinP2pEndPoint()
		{
			if (Network == Network.Main)
			{
				return MainNetBitcoinP2pEndPoint;
			}
			else if (Network == Network.TestNet)
			{
				return TestNetBitcoinP2pEndPoint;
			}
			else if (Network == Network.RegTest)
			{
				return RegTestBitcoinP2pEndPoint;
			}
			else
			{
				throw new NotSupportedNetworkException(Network);
			}
		}

		public EndPoint GetBitcoinCoreRpcEndPoint()
		{
			if (Network == Network.Main)
			{
				return MainNetBitcoinCoreRpcEndPoint;
			}
			else if (Network == Network.TestNet)
			{
				return TestNetBitcoinCoreRpcEndPoint;
			}
			else if (Network == Network.RegTest)
			{
				return RegTestBitcoinCoreRpcEndPoint;
			}
			else
			{
				throw new NotSupportedNetworkException(Network);
			}
		}

		protected override bool TryEnsureBackwardsCompatibility(string jsonString)
		{
			try
			{
				var jsObject = JsonConvert.DeserializeObject<JObject>(jsonString);
				bool saveIt = false;

				var mainNetBitcoinCoreHost = jsObject.Value<string>("MainNetBitcoinCoreHost");
				var mainNetBitcoinCorePort = jsObject.Value<int?>("MainNetBitcoinCorePort");
				var testNetBitcoinCoreHost = jsObject.Value<string>("TestNetBitcoinCoreHost");
				var testNetBitcoinCorePort = jsObject.Value<int?>("TestNetBitcoinCorePort");
				var regTestBitcoinCoreHost = jsObject.Value<string>("RegTestBitcoinCoreHost");
				var regTestBitcoinCorePort = jsObject.Value<int?>("RegTestBitcoinCorePort");

				if (mainNetBitcoinCoreHost != null)
				{
					int port = mainNetBitcoinCorePort ?? Constants.DefaultMainNetBitcoinP2pPort;

					if (EndPointParser.TryParse(mainNetBitcoinCoreHost, port, out EndPoint ep))
					{
						MainNetBitcoinP2pEndPoint = ep;
						saveIt = true;
					}
				}

				if (testNetBitcoinCoreHost != null)
				{
					int port = testNetBitcoinCorePort ?? Constants.DefaultTestNetBitcoinP2pPort;

					if (EndPointParser.TryParse(testNetBitcoinCoreHost, port, out EndPoint ep))
					{
						TestNetBitcoinP2pEndPoint = ep;
						saveIt = true;
					}
				}

				if (regTestBitcoinCoreHost != null)
				{
					int port = regTestBitcoinCorePort ?? Constants.DefaultRegTestBitcoinP2pPort;

					if (EndPointParser.TryParse(regTestBitcoinCoreHost, port, out EndPoint ep))
					{
						RegTestBitcoinP2pEndPoint = ep;
						saveIt = true;
					}
				}

				return saveIt;
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Backwards compatibility couldn't be ensured.");
				Logger.LogInfo(ex);
				return false;
			}
		}
	}
}
