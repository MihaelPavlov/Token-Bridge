import { BridgeContract__factory } from "./../typechain-types/factories/contracts/Bridge.sol/BridgeContract__factory";
import { BridgeContract } from "./../typechain-types/contracts/Bridge.sol/BridgeContract";
import { WrappedToken__factory } from "./../typechain-types/factories/contracts/Tokens/WrappedToken__factory";
import { WrappedToken } from "./../typechain-types/contracts/Tokens/WrappedToken";
import { MyToken__factory } from "./../typechain-types/factories/contracts/Tokens/MyToken__factory";
import { MyToken } from "./../typechain-types/contracts/Tokens/MyToken";
import { expect } from "chai";
import { ethers } from "hardhat";
import { access } from "../typechain-types/@openzeppelin/contracts";

describe("Bridge", function () {
    let bridgeFactory: BridgeContract__factory;
    let bridge: BridgeContract;

    let wrappedTokenFactory: WrappedToken__factory;
    let wrappedToken: WrappedToken;

    let myTokenFactory: MyToken__factory;
    let myToken: MyToken;

    before(async () => {
        bridgeFactory = await ethers.getContractFactory("BridgeContract");
        wrappedTokenFactory = await ethers.getContractFactory("WrappedToken");
        myTokenFactory = await ethers.getContractFactory("MyToken");

        wrappedToken = await wrappedTokenFactory.deploy("Wrapped", "W", 18);
        await wrappedToken.deployed();

        myToken = await myTokenFactory.deploy();
        await myToken.deployed();
    });

    beforeEach(async () => {
        bridge = await bridgeFactory.deploy()
        await bridge.deployed();
    })

    describe("lock", function () {

        it("Should transfer tokens to the contract", async () => {
            const [owner, addr1] = await ethers.getSigners();

            await myToken.approve(bridge.address, 100000000);

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);

            await bridge.lock(myToken.address, 100000);
            expect(await myToken.balanceOf(bridge.address)).to.equal(100000);
        });

        it("Should transfer tokens successfully and emit event lock", async () => {
            const [owner, addr1] = await ethers.getSigners();

            await myToken.approve(bridge.address, 100000000);

            await expect(await bridge.lock(myToken.address, 100000)).to.emit(bridge, "Lock").withArgs(myToken.address, owner.address, 100000);
        });

        it("Unsuccessfully token transfer(error: InsufficientAllowance)", async () => {
            const [owner, addr1] = await ethers.getSigners();

            await myToken.approve(bridge.address, 100000);

            await expect(bridge.lock(myToken.address, 1000001)).to.revertedWith("ERC20: insufficient allowance")
        });
    });

    describe("unlock", function () {
        it("Should transfer successfully tokens from bridge to addrress", async () => {
            await myToken.approve(bridge.address, 100000);

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);

            await bridge.lock(myToken.address, 100000);

            expect(await myToken.balanceOf(bridge.address)).to.equal(100000);

            await bridge.unlock("txHash", myToken.address, 100000);

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);
        })

        it("Should transfer successfully and emit event", async () => {
            const [owner, addr1] = await ethers.getSigners();
            await myToken.approve(bridge.address, 100000);

            await bridge.lock(myToken.address, 100000);

            await expect(await bridge.unlock("txHash", myToken.address, 100000)).to.emit(bridge, "Unlock").withArgs("txHash", myToken.address, owner.address, 100000);
        })

        it("Unsuccessfully transfer(error: TransferAmountExceedsBalance)", async () => {
            await expect(bridge.unlock("txHash", myToken.address, 100000)).to.revertedWith("ERC20: transfer amount exceeds balance");

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);
        })
    })

    describe("mint", function () {

        it("Successfully assigned mappings if we don't have corresponding token", async () => {
            expect(await bridge.sourceToWrappedTokenMap(myToken.address)).to.equal("0x0000000000000000000000000000000000000000");
            const tx = await bridge.mint("txHash", myToken.address, 100000, "TestToken", "TT");
            const receipt = await tx.wait();

            const event = receipt.events?.find(e => e.event === "Mint");
            if (event) {
                const { txHash, token, to, amount } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);

                expect(await bridge.sourceToWrappedTokenMap(myToken.address)).to.equal(token);
                expect(await bridge.wrappedToSourceTokenMap(token)).to.equal(myToken.address);
            }
            else {
                expect(event).to.not.be.undefined;
            }
        });

        it("Successfully emit event for (Created WrappedToken)", async () => {
            await expect(await bridge.mint("txHash", myToken.address, 100000, "TestToken", "TT")).to.emit(bridge, "WrappedTokenCreated");
        });

        it("Successfully emit event for (Mint)", async () => {
            await expect(await bridge.mint("txHash", myToken.address, 100000, "TestToken", "TT")).to.emit(bridge, "Mint");
        });

        it("Successfully mint tokens to the sender", async () => {
            const [owner, addr1] = await ethers.getSigners();
            const tx = await bridge.connect(addr1).mint("txHash", myToken.address, 1, "TestToken", "TT");
            const receipt = await tx.wait();

            const event = receipt.events?.find(e => e.event === "Mint");

            if (event) {
                const { txHash, token, to, amount } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);

             expect(await wrappedToken.attach(token).balanceOf(addr1.address)).to.equal("1000000000000000000");
             expect(amount).to.equal("1");
            }

            await bridge.connect(addr1).mint("txHash", myToken.address, 1, "TestToken", "TT")
        });
    });
})