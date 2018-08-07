// The following sample code uses modern ECMAScript 6 features 
// that aren't supported in Internet Explorer 11.
// To convert the sample for environments that do not support ECMAScript 6, 
// such as Internet Explorer 11, use a transpiler such as 
// Babel at http://babeljs.io/. 
//
// See Es5-chat.js for a Babel transpiled version of the following code:
	
function initSW(){
	if ('serviceWorker' in navigator && 'PushManager' in window) {	
		navigator.serviceWorker.register('sw.js')
		.then(function(swReg) {
			console.log('Service Worker is registered', swReg)
		})
		.catch(function(error) {
			console.error('Service Worker Error', error);
		});
	} else {
	  console.warn('Push messaging is not supported');
	}
}

initSW();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/MessageHub")
    .build();
			
connection.on("ReceiveMessage", (message, topic) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = msg;

    if (window.Notification) {
		Notification.requestPermission(function(result) {
			console.log('status: ', result);
			if (result === 'granted') {
				console.log(navigator.serviceWorker.ready);
				navigator.serviceWorker.ready.then(function(registration) {
				  registration.showNotification(topic, { body: encodedMsg });
				});
			}
		});
    }
    else {
        alert('your browser doesn\'t support notifications.');
    }
});

connection.start().catch(err => console.error(err.toString()));

//document.getElementById("sendButton").addEventListener("click", event => {
//    const user = document.getElementById("userInput").value;
//    const message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
//    event.preventDefault();
//});