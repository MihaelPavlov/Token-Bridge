import { BridgeContract__factory } from "./../typechain-types/factories/contracts/Bridge.sol/BridgeContract__factory";
import { BridgeContract } from "./../typechain-types/contracts/Bridge.sol/BridgeContract";
import { WrappedToken__factory } from "./../typechain-types/factories/contracts/Tokens/WrappedToken__factory";
import { WrappedToken } from "./../typechain-types/contracts/Tokens/WrappedToken";
import { MyToken__factory } from "./../typechain-types/factories/contracts/Tokens/MyToken__factory";
import { MyToken } from "./../typechain-types/contracts/Tokens/MyToken";
import { HubToken__factory } from "./../typechain-types/factories/contracts/Tokens/HubToken__factory";
import { HubToken } from "./../typechain-types/contracts/Tokens/HubToken";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Bridge", function () {
    let bridgeFactory: BridgeContract__factory;
    let bridge: BridgeContract;

    let wrappedTokenFactory: WrappedToken__factory;
    let wrappedToken: WrappedToken;

    let myTokenFactory: MyToken__factory;
    let myToken: MyToken;

    let hubTokenFactory: HubToken__factory;
    let hubToken: HubToken;

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

    it("hubToken", async () => {
        const [owner] = await ethers.getSigners();

        hubTokenFactory = await ethers.getContractFactory("HubToken");
        hubToken = await hubTokenFactory.deploy();
        await hubToken.deployed();

        await hubToken.mint(owner.address);

        expect(await hubToken.balanceOf(owner.address)).to.be.equal(2000000000);
    })

    describe("lock", function () {

        it("Should transfer tokens to the contract", async () => {
            await myToken.approve(bridge.address, 100000000);

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);

            await bridge.lock(myToken.address, 100000);
            expect(await myToken.balanceOf(bridge.address)).to.equal(100000);
        });

        it("Should transfer tokens successfully and emit event lock", async () => {
            const [owner] = await ethers.getSigners();
            await myToken.mint(owner.address);
            await myToken.approve(bridge.address, 100000000);

            await expect(await bridge.lock(myToken.address, 100000)).to.emit(bridge, "Lock").withArgs(myToken.address, owner.address, 100000);
        });

        it("Unsuccessfully token transfer(error: InsufficientAllowance)", async () => {
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
            const [owner] = await ethers.getSigners();
            await myToken.approve(bridge.address, 100000);

            await bridge.lock(myToken.address, 100000);

            await expect(await bridge.unlock("txHash", myToken.address, 100000)).to.emit(bridge, "Unlock").withArgs("txHash", myToken.address, owner.address, 100000);
        })

        it("Unsuccessfully transfer(error: TransferAmountExceedsBalance)", async () => {
            await expect(bridge.unlock("txHash", myToken.address, 100000)).to.revertedWith("ERC20: transfer amount exceeds balance");

            expect(await myToken.balanceOf(bridge.address)).to.equal(0);
        })

        it("Should throw error when try unlock with the same txHash two times)", async () => {
            await myToken.approve(bridge.address, 1000000);

            await bridge.lock(myToken.address, 100000);

            await bridge.unlock("txHash", myToken.address, 10000);
            await expect(bridge.unlock("txHash", myToken.address, 10000)).to.revertedWith("The transaction hash was already claimed");
        })
    })

    describe("mint", function () {

        it("Successfully assigned mappings if we don't have corresponding token", async () => {
            expect(await bridge.sourceToWrappedTokenMap(myToken.address)).to.equal("0x0000000000000000000000000000000000000000");
            const tx = await bridge.mint("txHash", myToken.address, 100000, "TestToken", "TT", 18);
            const receipt = await tx.wait();

            const event = receipt.events?.find(e => e.event === "Mint");
            if (event) {
                const { txHash, token, to, amount } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);

                expect(await bridge.sourceToWrappedTokenMap(myToken.address)).to.equal(token);
                expect(await bridge.wrappedToSourceTokenMap(token)).to.equal(myToken.address);
                expect(wrappedToken.attach(token).decimals(), "18");
            }
            else {
                expect(event).to.not.be.undefined;
            }
        });

        it("Successfully emit event for (Created WrappedToken)", async () => {
            await expect(await bridge.mint("txHash", myToken.address, 1, "TestToken", "TT", 18)).to.emit(bridge, "WrappedTokenCreated");
        });

        it("Successfully emit event for (Mint)", async () => {
            await expect(await bridge.mint("txHash", myToken.address, 1, "TestToken", "TT", 18)).to.emit(bridge, "Mint");
        });

        it("Successfully mint tokens to the sender", async () => {
            const [owner, addr1] = await ethers.getSigners();
            const tx = await bridge.connect(addr1).mint("txHash", myToken.address, 1000, "TestToken", "TT", 18);
            const receipt = await tx.wait();

            const event = receipt.events?.find(e => e.event === "Mint");

            if (event) {
                const { txHash, token, to, amount } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);

                expect(await wrappedToken.attach(token).balanceOf(addr1.address)).to.equal("1000");
                expect(amount).to.equal("1000");
            }

            await bridge.connect(addr1).mint("txHash2", myToken.address, 1, "TestToken", "TT", 18)
        });

        it("Should throw error when try mint with the same txHash two times", async () => {
            await bridge.mint("txHash", myToken.address, 1000, "TestToken", "TT", 18)
            await expect(bridge.mint("txHash", myToken.address, 1000, "TestToken", "TT", 18)).to.revertedWith("The transaction hash was already claimed");
        });
    });

    describe("burn", function () {

        it("Should be unsuccessfully and return error", async () => {
            await expect(bridge.burn("0x0000000000000000000000000000000000000000", 1)).to.revertedWith("There is no wrapped token with this address")
        });

        it("Successfully removed tokens from burn", async () => {
            const [, addr1] = await ethers.getSigners();

            const tx = await bridge.connect(addr1).mint("txHash", myToken.address, 1000, "TestToken", "TT", 18);
            const receipt = await tx.wait();
            const event = receipt.events?.find(e => e.event === "Mint");

            if (event) {
                const { token } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);
                expect(await bridge.connect(addr1).sourceToWrappedTokenMap(myToken.address)).to.equal(token);
                expect(await bridge.connect(addr1).wrappedToSourceTokenMap(token)).to.equal(myToken.address);
                expect(await wrappedToken.attach(token).balanceOf(addr1.address)).to.equal("1000");
                await expect(await bridge.connect(addr1).burn(token, 100)).to.emit(bridge, "Burn").withArgs(myToken.address, addr1.address, 100);
                expect(await wrappedToken.attach(token).balanceOf(addr1.address)).to.equal(900);
            }
        });

        it("Should successfully burn tokens and emit event", async () => {
            const [, addr1] = await ethers.getSigners();

            const tx = await bridge.connect(addr1).mint("txHash", myToken.address, 1, "TestToken", "TT", 18);
            const receipt = await tx.wait();
            const event = receipt.events?.find(e => e.event === "Mint");

            if (event) {
                const { token } = bridge.interface.decodeEventLog("Mint", event.data, event.topics);

                expect(await bridge.connect(addr1).sourceToWrappedTokenMap(myToken.address)).to.equal(token);

                await expect(await bridge.connect(addr1).burn(token, 1)).to.emit(bridge, "Burn").withArgs(myToken.address, addr1.address, 1);
            }
        });
    });
})