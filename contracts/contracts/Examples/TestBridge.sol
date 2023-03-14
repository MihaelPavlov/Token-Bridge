// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

import "hardhat/console.sol";

// for test signature https://cryptomarketpool.com/how-to-sign-verify-an-ethereum-message-off-chain/

contract TestBridge {
    //saving is the nonce processed.
    // Or i can just save the max nonce and to check if someone send me lower nonce then my max it mean that transfer maybe was already processed
    mapping(address => mapping(uint256 => bool)) processedNonces;

    // parameters used inside the emitted event
    function Lock (address to ,uint amount, uint nonce, bytes calldata signature) public{ // this nonce will be send and use in the mint function
        require(processedNonces[msg.sender][nonce]== false , "transfer already processed");
        processedNonces[msg.sender][nonce] = true;

        console.log("lock");
    }

    function Mint(address from, address to, uint amount, uint nonce, bytes calldata signature) public view{
        bytes32 message = getEthSignedMessageHash(keccak256(abi.encodePacked(from,to,amount,nonce)));

        require(recoverSigner(message,signature)== from , "wrong signature");
        require(processedNonces[from][nonce] == false, "transfer already processed");

        console.log("mint");
    }

    function getMessageHash(
        // function from outside:  create the hash and sign it
        address _from,
        address _to,
        uint256 _amount,
        uint256 _nonce
    ) public pure returns (bytes32) {
        return keccak256(abi.encodePacked(_from,_to, _amount, _nonce));
    }

    function verify(
        address _signer,
        address _to,
        uint256 _amount,
        uint256 _nonce,
        bytes memory signature
    ) public pure returns (bool) {
        bytes32 messageHash = getMessageHash(_signer,_to, _amount, _nonce);
        bytes32 ethSignedMessageHash = getEthSignedMessageHash(messageHash);

        return recoverSigner(ethSignedMessageHash, signature) == _signer;
    }

    function getEthSignedMessageHash(bytes32 _messageHash)
        public
        pure
        returns (bytes32)
    {
        /*
        Signature is produced by signing a keccak256 hash with the following format:
        "\x19Ethereum Signed Message\n" + len(msg) + msg
        */
        return
            keccak256(
                abi.encodePacked(
                    "\x19Ethereum Signed Message:\n32",
                    _messageHash
                )
            );
    }

    function recoverSigner(
        bytes32 _ethSignedMessageHash,
        bytes memory _signature
    ) public pure returns (address) {
        (bytes32 r, bytes32 s, uint8 v) = splitSignature(_signature);

        return ecrecover(_ethSignedMessageHash, v, r, s);
    }

    function splitSignature(bytes memory sig)
        public
        pure
        returns (
            bytes32 r,
            bytes32 s,
            uint8 v
        )
    {
        require(sig.length == 65, "invalid signature length");

        assembly {
            /*
            First 32 bytes stores the length of the signature

            add(sig, 32) = pointer of sig + 32
            effectively, skips first 32 bytes of signature

            mload(p) loads next 32 bytes starting at the memory address p into memory
            */

            // first 32 bytes, after the length prefix
            r := mload(add(sig, 32))
            // second 32 bytes
            s := mload(add(sig, 64))
            // final byte (first byte of the next 32 bytes)
            v := byte(0, mload(add(sig, 96)))
        }

        // implicitly return (r, s, v)
    }
}
