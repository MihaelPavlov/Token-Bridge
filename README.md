# Token-Bridge
 EVM Token bridge. It should allow users to bridge any ERC-20 token between two EVM compatible networks.
 
 # Links
 - Demo Video WeTransfer -> https://we.tl/t-xv9ylh9Ey8 
 - Demo Video One Drive -> https://1drv.ms/u/s!AtMi_LCoBbeMduWHj8lsCQBYFMA?e=eiejFv
 ### Ethereum Sepolia 
 - Bridge Contract -> https://sepolia.etherscan.io/address/0x1d5cB0D34648E44bd37d0a352ca806039C6963c8
 - Token -> https://sepolia.etherscan.io/address/0x3392BD8B4E411224D80c50D76e3b09Cb14BB79E0
 ### Binance Testnet 
 - Bridge Contract-> https://testnet.bscscan.com/address/0x447f756adf7155236cc3c87df5e27d03da7cb826
 - Token -> https://testnet.bscscan.com/address/0x4b32676537f12ae63507458a8e16aad17ac73bae
 
 # 📝Parts of the project
 - Solidity Contracts
 - CLI(Command Line Tool) 
 - BackEnd
 
 # 🔨 Used technologies
 - Hardhat
 - Solidity
 - ASP.NET Core
 - Entity Framework Core
 - MSSQL Server
 - Chai (testing)
 
 # Parts Explanation
 
 ### Bridge Contract
 Solidity smart contract that facilitates the transfer of tokens between different blockchain networks. 
 
 The contract allows locking and unlocking of tokens from the source network to the target network by minting and burning WrappedTokens, which are ERC20 tokens that    represent the original tokens locked on the source network. 
 
 The contract includes functions for locking, unlocking, minting, and burning tokens, as well as creating WrappedTokens. 
 
 The contract emits events for all of these actions. The contract also includes mappings to keep track of WrappedTokens and transactions that have been claimed. 
 
 The contract imports OpenZeppelin libraries for ERC20 tokens and uses SafeERC20 to prevent common ERC20 vulnerabilities.

 ### CLI 
C# program that connects to a smart contract deployed on the Ethereum Testnet or Binance Smart Chain Testnet blockchain. The program prompts the user to select the blockchain, and then it allows the user to perform one of four actions on the smart contract: Lock, Unlock, Mint, or Burn.

- The Lock function prompts the user to enter a source token and an amount to lock.
- The Unlock function prompts the user to enter a transaction hash, an original token, and an amount to unlock.
- The Mint function prompts the user to enter a transaction hash, a source token, and an amount to mint. 
- The Burn function prompts the user to enter a target token and an amount to burn.

Overall, this program allows the user to interact with the smart contract through a console-based interface.

### BackEnd
At the back-end we have two background services that are listening for events.

The servicethat listens for events on a blockchain network and saves them to a database. 

It uses the Nethereum library to connect to the network and fetch events, and the Entity Framework Core library to interact with the database. 

The main logic of the program is contained in the ExecuteAsync method, which runs indefinitely and listens for new events while periodically checking for any missed events. 

The program is designed to handle four different types of events, which it filters and processes asynchronously in parallel.

The CheckForEvents method is responsible for determining where to start fetching events based on whether there are any events already saved in the database. 
Overall, the program is designed to be fault-tolerant and capable of handling any errors that may occur.

