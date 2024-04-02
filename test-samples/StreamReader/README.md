This test sample receives a lot of streaming data.

Start the streaming device as:
```
cd testDevice
npm install
PRODUCT_ID=... DEVICE_ID=... PRIVATE_KEY=...  npx ts-node testDevice.ts
```

Create a private key with the edge key tool and upload the fingerprint.

Start run the dotnet sample as:
```
dotnet run -- -p ... -d ...
```

The sample will receive a lot of streaming data until it is closed.
