// 处理页面的动画
export function triggerAnimation(elementId, animationName = "fade-in") {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.remove(animationName);
        void element.offsetWidth;
        element.classList.add(animationName);
    }
    
};

// 用于脱离Blazor环境进行表单提交
export function postForm(requestAddr, model, antiForgeryToken) {
    // 创建一个新的表单
    var form = document.createElement("form");
    form.method = "POST";
    form.action = requestAddr;

    // 创建一个新的输入字段来存储防伪造令牌
    var tokenInput = document.createElement("input");
    tokenInput.type = "hidden";
    tokenInput.name = "__RequestVerificationToken";
    tokenInput.value = antiForgeryToken;

    // 将防伪造令牌添加到表单中
    form.appendChild(tokenInput);

    if (model) {
        // 遍历模型对象的属性
        for (var key in model) {
            if (model.hasOwnProperty(key)) {
                // 创建一个新的输入字段
                var input = document.createElement("input");
                input.type = "hidden";
                input.name = key;
                input.value = model[key];

                // 将输入字段添加到表单中
                form.appendChild(input);
            }
        }
    }
    

    // 将表单添加到文档中
    document.body.appendChild(form);

    // 提交表单
    form.submit();
}

export function redirect(addr){
    window.location = addr;
}