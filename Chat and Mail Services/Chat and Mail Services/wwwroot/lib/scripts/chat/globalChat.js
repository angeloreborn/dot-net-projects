var initialized = false;
var currentUser = '';
var messageArea = document.getElementById('messageArea');
let mainPath = 'wss://localhost:5001';
let wssUrl = '/GlobalChats';
var socket = new WebSocket(mainPath.concat(wssUrl));
socket.onmessage = (event) => {
    if (initialized === false)
        throw Error("User not initialized");
    if (event instanceof WebSocket) {
        let webSocket = event;
    }
    else {
        let messageEv = event;
        let base64Segments = event.data.split('|');
        base64Segments.shift();
        let combinedMessage = {
            Author: {
                Email: '',
                ProfileImageUrl: 'default message'
            },
            Content: {
                Data: '',
            },
            SendDate: ''
        };
        for (let index = 0; index < base64Segments.length; index++) {
            let segment = atob(base64Segments[index]);
            let messageSegment = JSON.parse(segment);
            combinedMessage.Author.Email = messageSegment.Author.Email;
            combinedMessage.Author.ProfileImageUrl = messageSegment.Author.ProfileImageUrl;
            combinedMessage.Content.Data += messageSegment.Content.Data;
            combinedMessage.SendDate = getFormattedRelativeDate(messageSegment.SendDate);
        }
        let messageContent = {
            message: combinedMessage,
            author: combinedMessage.Author.Email,
            sendDate: combinedMessage.SendDate
        };
        renderMessage(messageContent);
    }
};
function getFormattedRelativeDate(dateTime) {
    let messageDate = new Date(dateTime);
    let date = messageDate.getDay();
    let hours = messageDate.getHours();
    let minutes = messageDate.getMinutes();
    let dayToday = new Date().getDay();
    let dateDifference = getDayDiffernce(dayToday, date);
    let relativeMessageTime = `${dateDifference} at ${messageDate.getHours()}:${messageDate.getMinutes()}`;
    return relativeMessageTime;
}
function getDayDiffernce(today, serverdate) {
    let dayDifference = today - serverdate;
    switch (dayDifference) {
        case 0: return 'Today';
        case 1: return 'Yesterday';
        default: return `${dayDifference} days ago`;
    }
}
let init = (user) => {
    currentUser = user;
    initialized = true;
};
let renderMessage = (messageContent) => {
    let messageContainer = document.createElement('div');
    messageContainer.className = 'message-container';
    messageContainer.innerHTML = createMessageBlockOrigin(messageContent);
    messageArea.appendChild(messageContainer);
};
socket.onopen = (e) => {
    console.log(e);
};
socket.onerror = (e) => {
    console.log(e);
};
let sendMessage = (userName) => {
    let messageInput = document.getElementById('messageInput');
    if (messageInput.innerHTML != '') {
        let inputString = messageInput.innerHTML.toString();
        socket.send(inputString);
        messageInput.innerHTML = '';
    }
};
function createMessageBlockOrigin(m) {
    return `<div class="image-container">
                    <img class="image" src='${window.location.origin}/images/thumbnail/${m.message.Author.ProfileImageUrl}.png'/>
                </div>
                <div class="message-content-container">
                    <div class="message-author">
                        ${m.message.Author.Email}
                        <small>
                            ${m.sendDate}
                        </small>
                    </div>
                    <div class="message-content">
                        <div class="span"></div>
                        <div class="message-text">
                          ${m.message.Content.Data}
                        </div>
                    </div>
              </div>`;
}
function createMessageBlockMember(m) {
    return `<div class="d-flex justify-content-end">
                   <div class="message-box position-relative">
                        <div class="message-user">${m.message.Author.Email}</div>
                        <div class="message-content">${m.message.Content.Data}</div>
                        <div class="message-time ml-auto">Today 00:00 am</div>
                    </div>
                    <div class="d-flex align-items-center">
                        <div class="message-image-container">
                            <img class="message-image" src="${window.location.origin}/images/thumbnail/${m.message.Author.ProfileImageUrl}.png" />
                        </div>
                    </div>
            </div>`;
}
//# sourceMappingURL=globalChat.js.map