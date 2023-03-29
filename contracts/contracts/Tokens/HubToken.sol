// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract HubToken is ERC20, Ownable {
    constructor() ERC20("HubToken", "HT") {
        _mint(msg.sender, 1000000000);
    }

    function mint(address _sender) public{
        _mint(_sender, 1000000000);
    }
}