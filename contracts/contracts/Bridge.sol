// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "./Tokens/WrappedToken.sol";

// Integration Tests or Unit Tests, 
// Tests with Echidna
// Static vs Dynamic Testing of contract
contract BridgeContract {
    event TokenLocked(
        address indexed token,
        address indexed sender,
        uint256 amount
    );

    event TokenUnlocked(
        string txHash,
        address indexed token,
        address indexed to,
        uint256 amount
    );

    event TokenMinted(
        string txHash,
        address indexed token,
        address indexed to,
        uint256 amount
    );

     event TokenBurned(
        address indexed token,
        address indexed sender,
        uint256 amount
    );
    
    event WrappedTokenCreated(
        address indexed wrappedToken,
        address indexed originalToken
    );

    event LogMessage(uint256 message);

    mapping(address => mapping(bytes32 => uint256)) public lockedTokens;
    mapping(address => address) public sourceToWrappedTokenMap;
    mapping(address => address) public wrappedToSourceTokenMap;

    function lock(address sourceToken, uint256 amount) public {
        // Helper for visualization
        uint256 value = IERC20(sourceToken).balanceOf(msg.sender); // sender balance
        string memory result = uint256ToString(value);

        require(IERC20(sourceToken).balanceOf(msg.sender) >= amount, result);
        require(
            IERC20(sourceToken).transferFrom(msg.sender, address(this), amount),
            "Transfer failed"
        );

        bytes32 tokenIdentifier = keccak256(abi.encodePacked(sourceToken));

        lockedTokens[msg.sender][tokenIdentifier] += amount;

        emit TokenLocked(sourceToken, msg.sender, amount);
    }

    function unlock(string memory txHash,address originalToken, uint256 amount) public {
        // bytes32 tokenIdentifier = keccak256(
        //     abi.encodePacked(wrappedToken)
        // );// debugging

        // lockedTokens[msg.sender][tokenIdentifier] -= amount; // debugging
        
        IERC20(originalToken).transfer(
            msg.sender,
            amount
        );

        emit TokenUnlocked(
            txHash,
            originalToken,
            msg.sender,
            amount
        );
    }

    // Mint when we don't have equivalent to the sourceToken
    function mint(
        string memory txHash,
        address sourceToken,
        uint256 amount,
        string memory name,
        string memory symbol
        //decimals
    ) public {
        if (sourceToWrappedTokenMap[sourceToken] == address(0)) {
            address wrappedToken = createWrappedToken(sourceToken,name,symbol);

            sourceToWrappedTokenMap[sourceToken] = wrappedToken;
            wrappedToSourceTokenMap[wrappedToken] = sourceToken;
        }

        mint(txHash,sourceToken,amount);
    }

    // Mint when we have equivalent to the sourceToken
    function mint(string memory txHash,address sourceToken, uint256 amount) public {
        WrappedToken(sourceToWrappedTokenMap[sourceToken]).mint(
            msg.sender,
            amount
        );

        emit TokenMinted(
            txHash,
            sourceToWrappedTokenMap[sourceToken],
            msg.sender,
            amount
        );
    }

    function burn(address wrappedToken, uint256 amount) public {
        WrappedToken wToken = WrappedToken(wrappedToken);
        require(address(wToken) != address(0), "There is no wrapped token with this address");
        wToken.burn(msg.sender, amount);
        emit TokenBurned(wrappedToSourceTokenMap[wrappedToken],msg.sender, amount);
    }

    function createWrappedToken(
        address originalToken,
        string memory name,
        string memory symbol
    ) internal returns (address) {
        WrappedToken wrappedToken = new WrappedToken(
            originalToken,
            string(abi.encodePacked("Wrapped", name)),
            string(abi.encodePacked("W", symbol))
        );

        // Mint the initial supply of the wrapped token to the Bridge Contract
        wrappedToken.mint(address(this), 1000000000000000000); // Question: Can this supply end ?

        emit WrappedTokenCreated(address(wrappedToken), originalToken);

        return address(wrappedToken);
    }

    // Add function for getting token details, MaxSupply, name , symbol and etc.
    function uint256ToString(uint256 _i) internal pure returns (string memory) {
        if (_i == 0) {
            return "0";
        }
        uint256 j = _i;
        uint256 length;
        while (j != 0) {
            length++;
            j /= 10;
        }
        bytes memory bstr = new bytes(length);
        uint256 k = length;
        while (_i != 0) {
            k = k - 1;
            uint8 temp = uint8(48 + uint8(_i % 10));
            bytes1 b1 = bytes1(temp);
            bstr[k] = b1;
            _i /= 10;
        }
        return string(bstr);
    }

    function getBytes(address token) public pure returns (bytes32) {
        return keccak256(abi.encodePacked(token));
    }
}
