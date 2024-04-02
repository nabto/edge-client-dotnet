import { NabtoDeviceFactory, Stream } from 'embedded-sdk-node'
import { randomBytes } from 'crypto'
import { setTimeout } from 'timers/promises'
import * as dotenv from "dotenv";

async function handleStream(stream: Stream) {
    try {
        await stream.accept();

        const arrayBuffer = new ArrayBuffer(1024*1024);
        const view = new Uint8Array(arrayBuffer);
        for (let i = 0; i < view.length; ++i) {
            view[i] = i;
        }

        while (true) {
            await stream.write(arrayBuffer)
            await setTimeout(1000)
        }
    } catch (e) {
        // ignore errors
        console.log(`This is probably an expected stream error ${e}`);
    }
}

async function runDevice(productId: string, deviceId: string, privateKey: string) {
    var device = NabtoDeviceFactory.create();
    device.setLogLevel("trace");
    device.setLogCallback((logMessage) => { console.log(`${logMessage.severity} - ${logMessage.message}`) })

    device.setOptions({ deviceId: deviceId, productId: productId });
    device.experimental.setRawPrivateKey(privateKey)

    console.log("starting device");
    await device.start()
    device.addServerConnectToken("demosct");


    device.addStream(1234, handleStream)
}


async function main() {
    if (process.env.DEVICE_ID != null && process.env.PRODUCT_ID != null) {
    } else {
        console.log("Missing DEVICE_ID and/or PRODUCT_ID environment variables");
        return;
    }

    if (process.env.PRIVATE_KEY != null) {
    } else {
        console.log("Missing PRIVATE_KEY environment variables");
        return;
    }

    await runDevice(process.env.PRODUCT_ID, process.env.DEVICE_ID, process.env.PRIVATE_KEY);
}

main()
