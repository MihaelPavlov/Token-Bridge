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

    function decimals() public view override returns (uint8) {
        return _decimals;
    }

    function mint(address account, uint256 amount) public {
        _mint(account, amount);
    }

    function burn(address account, uint256 amount) public {
        _burn(account, amount);
    }
}
