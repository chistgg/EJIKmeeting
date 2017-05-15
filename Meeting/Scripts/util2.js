

$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    $("chatcontent").fixHeight(600);
    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.addMessage = function (name, message, time) {
   //     var chatContent = $('#chatcontent');
        // Добавление сообщений на веб-страницу 
        $('#chatcontent').append('<p><b>' + htmlEncode(name)
        + '</b>: ' + htmlEncode(message) + '</br>' + time + '</p>');
      //  chatContetn.scrollTop(chatContent[0].scrollHeight - chatContent.height());
    };

    // Функция, вызываемая при подключении нового пользователя
    chat.client.onConnected = function (id, userName, allUsers) {
        $('#chatBody').show();
        // установка в скрытых полях имени и id текущего пользователя
        $('#hdId').val(id);


        // Добавление всех пользователей
        for (i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
    }

    // Добавляем нового пользователя
    chat.client.onNewUserConnected = function (id, name) {

        AddUser(id, name);
    }

    // Удаляем пользователя
    chat.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();
    }

    // Открываем соединение
    $.connection.hub.start().done(function () {

        Connecting();

        $('#sendmessage').click(function () {
            chat.server.send($('#username').val(), $('#chatId').val(), $('#message').val());
            $('#message').val('');
        });

   /*     $("#message").keydown(function (e) {
            if (e.keyCode == 13 && !e.shiftKey) {
                Send();
            }
        });
        */
    });
});
// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
//Добавление нового пользователя
function AddUser(id, name) {

    var userId = $('#hdId').val();

    if (userId !== id) {

        $("#chatusers").append('<p id="' + id + '"><b>' + name + '</b></p>');
    }
}

function Connecting() {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    chat.server.connect($('#username').val(), $('#chatId').val());
}


/*function Send() {
    var msgInput = $('#message');
    chat.server.send($('#username').val(), $('#chatId').val(), msgInput.val());
    msgInput.val('');
    msgInput.focus();
}

$.fn.fixHeight = function (callback) {
    var fixItem = $(this);

    $(window).bind('load', fix);
    $(window).bind('resize', fix);

    function fix() {
        var height = callback();
        fixItem.css('height', height);
    }
}*/