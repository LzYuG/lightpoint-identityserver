(() => {
    // ���Դ���
    const maximumRetryCount = 10000;

    // ���Լ��
    const retryIntervalMilliseconds = 1000;

    const startReconnectionProcess = () => {

        let isCanceled = false;

        (async () => {
            for (let i = 0; i < maximumRetryCount; i++) {
                console.log(`��ͼ��������: ${i + 1} of ${maximumRetryCount}`)
                await new Promise(resolve => setTimeout(resolve, retryIntervalMilliseconds));

                if (isCanceled) {
                    return;
                }

                try {
                    const result = await Blazor.reconnect();
                    if (!result) {
                        // �ѵ���������������ӱ��ܾ�;���¼���ҳ�档
                        location.reload();
                        return;
                    }

                    // �ɹ��������ӵ���������
                    return;
                } catch {
                    //û�е��������;����һ�Ρ�
                }
            }

            // ���Դ���̫��;���¼���ҳ�档
            location.reload();
        })();

        return {
            cancel: () => {
                isCanceled = true;
            },
        };
    };

    let currentReconnectionProcess = null;

    Blazor.start({
        configureSignalR: function (builder) {
            let c = builder.build();
            c.serverTimeoutInMilliseconds = 30000;
            c.keepAliveIntervalInMilliseconds = 15000;
            builder.build = () => {
                return c;
            };
        },
        reconnectionHandler: {
            onConnectionDown: () => currentReconnectionProcess ??= startReconnectionProcess(),
            onConnectionUp: () => {
                currentReconnectionProcess?.cancel();
                currentReconnectionProcess = null;
            },
        },
    });
})();