﻿using System.ComponentModel;
using Chaincase.Models;
using Newtonsoft.Json;
using WalletWasabi.Bases;

namespace Chaincase
{
    [JsonObject(MemberSerialization.OptIn)]
	public class UiConfig : ConfigBase
	{
        private string _balance;
		private TransactionInfo[] _transactions;
		private bool _hasSeed;
		private bool _isBackedUp;

		[DefaultValue("0")]
		[JsonProperty(PropertyName = "Balance", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string Balance
		{
			get => _balance;
			set => RaiseAndSetIfChanged(ref _balance, value);
		}

		[JsonProperty(PropertyName = "Transactions")]
		public TransactionInfo[] Transactions
		{
			get => _transactions;
			set => RaiseAndSetIfChanged(ref _transactions, value);
		}

		[JsonProperty(PropertyName="HasSeed")]
		public bool HasSeed
		{
			get => _hasSeed;
			set => RaiseAndSetIfChanged(ref _hasSeed, value);
		}

        [JsonProperty(PropertyName="IsBackedUp")]
        public bool IsBackedUp
        {
			get => _isBackedUp;
			set => RaiseAndSetIfChanged(ref _isBackedUp, value);
        }
    
		public UiConfig() : base()
		{
		}

		public UiConfig(string filePath) : base(filePath)
		{
		}
	}
}
