// SPDX-License-Identifier: UNLICENSED

pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/draft-ERC20Permit.sol";
import "./Tokens/WrappedToken.sol";

// Integration Tests or Unit Tests,
// Tests with Echidna
// Static vs Dynamic Testing of contract
contract BridgeContract {
    using SafeERC20 for IERC20;

    event Lock(address indexed token, address indexed sender, uint256 amount);

    event Unlock(
        string txHash,
        address indexed token,
        address indexed to,
        uint256 amount
    );

    event Mint(
        string txHash,
        address indexed token,
        address indexed to,
        uint256 amount
    );

    event Burn(address indexed token, address indexed sender, uint256 amount);

    event WrappedTokenCreated(
        address indexed wrappedToken,
        address indexed originalToken
    );

    mapping(address => address) public sourceToWrappedTokenMap;
    mapping(address => address) public wrappedToSourceTokenMap;
    mapping(string => bool) public claimedTransactions;

    function lock(address sourceToken, uint256 amount) public // uint8 v,
    // bytes32 r,
    // bytes32 s
    {
        // ERC20Permit(sourceToken).permit(
        //     msg.sender,
        //     address(this),
        //     amount,
        //    2661766724,
        //     v,
        //     r,
        //     s
        // );
        IERC20(sourceToken).safeTransferFrom(msg.sender, address(this), amount);

        emit Lock(sourceToken, msg.sender, amount);
    }

    function unlock(
        string memory txHash,
        address originalToken,
        uint256 amount
    ) public {
        require(
            claimedTransactions[txHash] == false,
            "The transaction hash was already claimed"
        );

        IERC20(originalToken).safeTransfer(msg.sender, amount);

        claimedTransactions[txHash] = true;

        emit Unlock(txHash, originalToken, msg.sender, amount);
    }

    function mint(
        string memory txHash,
        address sourceToken,
        uint256 amount,
        string memory name,
        string memory symbol
    ) public {
        require(
            claimedTransactions[txHash] == false,
            "The transaction hash was already claimed"
        );

        if (sourceToWrappedTokenMap[sourceToken] == address(0)) {
            ERC20 token = ERC20(sourceToken);
            createWrappedToken(sourceToken, name, symbol, token.decimals());
        }

        WrappedToken(sourceToWrappedTokenMap[sourceToken]).mint(
            msg.sender,
            amount
        );

        claimedTransactions[txHash] = true;

        emit Mint(
            txHash,
            sourceToWrappedTokenMap[sourceToken],
            msg.sender,
            amount
        );
    }

    function burn(address wrappedToken, uint256 amount) public {
        WrappedToken wToken = WrappedToken(wrappedToken);
        require(
            address(wToken) != address(0),
            "There is no wrapped token with this address"
        );
        wToken.burn(msg.sender, amount);
        emit Burn(wrappedToSourceTokenMap[wrappedToken], msg.sender, amount);
    }

    function createWrappedToken(
        address sourceToken,
        string memory name,
        string memory symbol,
        uint8 decimals
    ) internal returns (address) {
        WrappedToken wrappedToken = new WrappedToken(
            string(abi.encodePacked("Wrapped", name)),
            string(abi.encodePacked("W", symbol)),
            decimals
        );

        address wrappedTokenAddress = address(wrappedToken);

        sourceToWrappedTokenMap[sourceToken] = wrappedTokenAddress;
        wrappedToSourceTokenMap[wrappedTokenAddress] = sourceToken;

        emit WrappedTokenCreated(wrappedTokenAddress, sourceToken);

        return wrappedTokenAddress;
    }
}
