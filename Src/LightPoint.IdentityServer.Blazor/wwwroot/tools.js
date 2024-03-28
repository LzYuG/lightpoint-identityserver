// ����ҳ��Ķ���
export function triggerAnimation(elementId, animationName = "fade-in") {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.remove(animationName);
        void element.offsetWidth;
        element.classList.add(animationName);
    }
    
};

// ��������Blazor�������б��ύ
export function postForm(requestAddr, model, antiForgeryToken) {
    // ����һ���µı�
    var form = document.createElement("form");
    form.method = "POST";
    form.action = requestAddr;

    // ����һ���µ������ֶ����洢��α������
    var tokenInput = document.createElement("input");
    tokenInput.type = "hidden";
    tokenInput.name = "__RequestVerificationToken";
    tokenInput.value = antiForgeryToken;

    // ����α��������ӵ�����
    form.appendChild(tokenInput);

    if (model) {
        // ����ģ�Ͷ��������
        for (var key in model) {
            if (model.hasOwnProperty(key)) {
                // ����һ���µ������ֶ�
                var input = document.createElement("input");
                input.type = "hidden";
                input.name = key;
                input.value = model[key];

                // �������ֶ���ӵ�����
                form.appendChild(input);
            }
        }
    }
    

    // ������ӵ��ĵ���
    document.body.appendChild(form);

    // �ύ��
    form.submit();
}

export function redirect(addr){
    window.location = addr;
}