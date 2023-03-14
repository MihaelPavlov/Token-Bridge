//using System;
//using System.Numerics;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Hosting;
//using Nethereum.ABI.FunctionEncoding.Attributes;
//using Nethereum.Contracts;
//using Nethereum.Contracts.Extensions;
//using Nethereum.RPC.Eth.DTOs;
//using Nethereum.Web3;

//namespace Bridge_Project
//{
//    public class EventService : BackgroundService
//    {
//        private readonly string _tokenContractAddress;
//        private readonly string _accountAddress;
//        private readonly Web3 _web3;


//        public EventService(string tokenContractAddress, string accountAddress, string bscEndpointUrl)
//        {
//            _tokenContractAddress = tokenContractAddress;
//            _accountAddress = accountAddress;
//            _web3 = new Web3(bscEndpointUrl);
//        }
//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var tokenContract = new Contract(null, _tokenContractAddress, _web3.Eth.GetContractABI(ContractHelper.("ERC20")));
//            var transferEvent = tokenContract.GetEvent<TransferEvent>();
//            var filter = transferEvent.CreateFilterInput(new BlockParameter(BigInteger.Zero), new BlockParameter("latest"));

//            var transferEventSubscription = transferEvent.CreateFilterObservable(filter).Subscribe(log =>
//            {
//                var transferEventDto = transferEvent.DecodeEvent(log);
//                if (transferEventDto.Event.From == _accountAddress || transferEventDto.Event.To == _accountAddress)
//                {
//                    Console.WriteLine($"Transfer: From={transferEventDto.Event.From}, To={transferEventDto.Event.To}, Value={transferEventDto.Event.Value}");
//                }
//            });

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                await Task.Delay(1000, stoppingToken);
//            }

//            transferEventSubscription.Dispose();
//        }
//    }
//}
