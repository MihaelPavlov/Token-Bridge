// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

contract WrappedToken is ERC20 {
    uint8 private _decimals;

    constructor(
        string memory _name,
        string memory _symbol,
        uint8 decimals
    ) ERC20(_name, _symbol) {
        _decimals = decimals;
    }

    function mint(address account, uint256 amount) public {
        _mint(account, amount * 10**uint256(_decimals));
    }

    function burn(address account, uint256 amount) public {
        _burn(account, amount * 10**uint256(_decimals));
    }
}
