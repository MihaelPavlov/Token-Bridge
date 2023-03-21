import { HardhatUserConfig } from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";

require('dotenv').config();

const { BSC_NETWORK_URL, INFURA_URL_SEPOLIA, PRIVATE_KEY, PRIVATE_BSC_KEY } = process.env;

const config: HardhatUserConfig = {
  solidity: "0.8.18",
  networks: {
    bscTestnet: {
      url: BSC_NETWORK_URL,
      chainId: 97,
      accounts: [`0x${PRIVATE_BSC_KEY}`],
    },
    sepolia: {
      url: INFURA_URL_SEPOLIA,
      accounts: [`0x${PRIVATE_KEY}`],
    }
  },
};

export default config;
