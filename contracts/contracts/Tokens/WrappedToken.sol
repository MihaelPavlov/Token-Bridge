// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

contract WrappedToken is ERC20 {
    address public originalToken;

    constructor(
        address _originalToken,
        string memory _name,
        string memory _symbol
    ) ERC20(_name, _symbol) {
        originalToken = _originalToken;
    }

    function mint(address account, uint256 amount) public {
        _mint(account, amount);
    }

    function burn(address account, uint256 amount) public {
        _burn(account, amount);
    }

    function getOriginalToken() public view returns (address) {
        return originalToken;
    }
}
