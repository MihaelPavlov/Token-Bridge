import { ethers } from "hardhat";
require('dotenv').config();

const { PUBLIC_KEY_SEPOLIA } = process.env;

async function main() {
  console.log("test -> ", PUBLIC_KEY_SEPOLIA);

  const deployer = ethers.provider.getSigner(PUBLIC_KEY_SEPOLIA);
  console.log(`Deploying contract with the account: ${deployer._address}`);

  const Bridge_Factory = await ethers.getContractFactory("BridgeContract");
  const bridge = await Bridge_Factory.deploy();

  await bridge.deployed();

  console.log(
    `Bridge is deployed to address ${bridge.address}`
  );
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
