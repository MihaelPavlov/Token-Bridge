// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract DinoToken is ERC20, Ownable {
    constructor() ERC20("DinoToken", "DT") {
        _mint(msg.sender, 10000000000);
    }

    function mint(address _sender) public{
        _mint(_sender,10000000000);
    }
}