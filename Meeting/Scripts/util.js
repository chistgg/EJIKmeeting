

$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    var allowsending = true;
    var isprotocolshowed = false;
    var fileid = "";

    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.addMessage = function (name, message, time, fid, fname, inprot) {
        var cssclass = inprot ? 'continprot' : 'cont';
        var content = '<p class="'+ cssclass +'"><b>' + htmlEncode(name) + '</b> '
        + (htmlEncode(time) + ' :')
        + '</br>' + htmlEncode(message) + '<br/>';

        if (fid !== "")
        {
            var fileurl;
            if (fid === '0') 
            {
                fileurl = $("#protdownloadlink").val();
            }
            else {
                fileurl = $("#downloadlink").val() + "?id=" + fid;
            }
           content += ('<a href="' + fileurl + '" target="_blank">' + fname + '</a><br/></p>');   //передавать параметром изображение в зависимости от типа          
        }
        // Добавление сообщений на веб-страницу 
        $('#chatcontent').prepend(content);
    };

    function addCommandMessage(message) {
        $('#chatcontent').prepend('<p class="leftcont"><b> Info' + '</b>: ' + htmlEncode(message) + '</br></p>');
    }

    chat.client.onChatStop = function () {
        hideInputForm();
        addCommandMessage('Совещание остановлено.');
    };

    chat.client.onChatContinue = function () {
        showInputForm();
        addCommandMessage('Совещание продолжено.');
    };

    // Функция, вызываемая при подключении нового пользователя
    chat.client.onConnected = function (id, userName, isShouldVote, allUsers) {
        $('#chatBody').show();

        if (isShouldVote)
            $('#confirmForm').show();

        SetStatus($('#chatStatus').val()); //TODO для того чтоб сначала не отображались обе кнопки статуста, возможно нужно поместить функцию выше.

        // установка id текущего пользователя
        $('#connectionID').val(id);

        // Добавление всех пользователей
        for (i = 0; i < allUsers.length; i++) {

            if (id !== allUsers[i].ConnectionID) {
                $("#chatusers").append('<div id="' + allUsers[i].ConnectionID + '">' + allUsers[i].Nick + '<div class=votestat>' + getProtocolConfirmatinString(allUsers[i].Status) + '</div></br></div>');
            } else {
                $("#chatusers").append('<div id="' + allUsers[i].ConnectionID + '"><b>' + allUsers[i].Nick + '</b><div class=votestat>' + getProtocolConfirmatinString(allUsers[i].Status) + '</div></br></div>');
            }
        }
    }

    // Добавляем нового пользователя
    chat.client.onNewUserConnected = function (id, userName, status) {

        if ($('#connectionID').val() !== id && !($("#" + id).length)) 
        {
            $("#chatusers").append('<div id="' + id + '">' + userName + '<div class=votestat>' + getProtocolConfirmatinString(status) + '</div></br> </div>');
        }

        sendCommandMessage(userName + ' присоединился к совещанию.');
    }

    // Удаляем пользователя
    chat.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();

        addCommandMessage(userName + ' покинул совещание.');
    }

    chat.client.onVote = function (id, vote) {
        $("#" + id).find($("div.votestat")).append(vote);
    }

    chat.client.onConfirmProtocol = function ()
    {
        hideInputForm();
        resetVote();
    }

    chat.client.onSendNewProtocol = function ()
    {
        hideInputForm();
        showConfirmForm();
        resetVote();
    }

    // Открываем соединение
    $.connection.hub.start().done(function () {

        Connecting();

        $('#sendmessage').click(function () {
            // Вызываем у хаба метод Send
            if (allowsending) {
                sendMsg($('#message').val(), fileid, $('#fileinfo').text(), false);        

            } else {
                setFileInfo("Дождитесь окончания загрузки", 2000);
            }
        });

        $('#sendmessageprot').click(function () {
            // Вызываем у хаба метод Send
            if (allowsending) {
                sendMsg($('#message').val(), fileid, $('#fileinfo').text(), true);

            } else {
                setFileInfo("Дождитесь окончания загрузки", 2000);
            }
        });

        $('#addvideo').click(function () {
            alert("в разработке.");
        });

        $("#message").keydown(function (e) {
            if (allowsending)
            {
            if (e.keyCode === 13 && !e.shiftKey) {
                sendMsg($('#message').val(), fileid, $('#fileinfo').text(), false);
            }
            }
            else {
                setFileInfo("Дождитесь окончания загрузки", 2000);
            }
        });

        $("#stopchat").click(function () {
            if (confirm("Вы уверены, что хотите завершить совещание?"))
            {
                sendMsg("_stopchat", "", "", false);
                return true;
            }
            else
                return false;
        });

        $("#activatechat").click(function () {
            if (confirm("Вы уверены, что хотите возобновить совещание?"))
            {
                sendMsg("_activatechat", "", "", false);
                return true;
            }   
            else
                return false;
        });

        $('#upload').change(function () {
            var files = this.files;

            if (window.FormData !== undefined) {
                fileLoadStart();

                var data = new FormData();
                $.each(files, function (key, value) {
                    data.append(key, value);
                });
                    $.ajax({
                        type: "POST",
                        url: $("#uploadlink").val(),
                        contentType: false,
                        processData: false,
                        data: data,
                        success: function (result) {
                            if (result === "") {
                                chooseFileReset("Формат файла не поддерживается или файл слишком большой (более 100 Мб)!", 4000);
                            }
                            else {
                                fileid = result[0];
                                setFileInfo(result[1], -1);
                                $('#delete').show();
                            }
                            fileLoadStop();
                        },
                        error: function (xhr, status, p3) {
                            chooseFileReset("Ошибка загрузки файла! " + xhr +" "+ status +" "+ p3 , 4000);
                            fileLoadStop();
                        }
                    });
                } else {
                    fileLoadStop();
                }
                
        });

        $('#delete').click(function () {
            $.get($("#deletelink").val(), { fn: fileid });

            chooseFileReset("deleted", 1);

        });

        $('#createprotocol').click(function () {
            if (isprotocolshowed) {
                hideProtocol();
            }
            else {
                showProtocol();
            }
        });

        $('#sendprotocol').click(function () {
            $.ajax({
                type: "POST",
                url: $("#crprotlink").val(),
                contentType: false,
                processData: false,
                data: "",
                success: function (result) {
                    sendMsg("_sendprotocol", "", "", false);
                    hideProtocol();
                },
                error: function (xhr, status, p3) {
                    alert("Ошибка создания протокола" + xhr + " " + status + " " + p3, 4000);
                }
            });
        });

        $('#confirm').click(function () {
            sendMsg("_confirm", "", "", false);
            $('#confirmForm').hide();
        });

        $('#reject').click(function () {
            sendMsg("_reject", "", "", false);
            $('#confirmForm').hide();
        });
       
    });

    function fileLoadStart()
    {
        $("#loadanim").show();
        allowsending = false;
    }

    function fileLoadStop()
    {
        $("#loadanim").hide();
        allowsending = true;
    }

    function chooseFileReset(msg, timeout)
    {
        fileid = "";
        $('#upload').val("");
        $('#delete').hide();
        
        if (msg !== "" && msg !== undefined)   //показать иконку ошибки загрзки файла
            setFileInfo(msg, timeout);
    }

    function setFileInfo(info, timeout) {
        $('#fileinfo').text(info);
        $('#fileinfo').show();

        if (timeout > 0) {
            window.setTimeout($('#fileinfo').hide(), timeout);
            $('#fileinfo').text("");
        }
    }

    function sendMsg(content, fileid, filename, toprotocol) {
        if (content !== "")
        {
            chat.server.send($('#connectionID').val(), content, fileid, filename, toprotocol);
            chooseFileReset("bb", 1);
            $('#message').val('');

            if ($('#message').selectionStart) {
                $('#message').setSelectionRange(0, 0);
                $('#message').focus();
            }
        }

    }

    function SetStatus(status) {
        switch (status[1]) {
            case '1':
            case '2':
                hideInputForm();
                break
            case '0':
                showInputForm();
                break
            default:
        }
    }

    function hideInputForm() {
        $('#inputForm').hide();
        $('#stopchat').hide();
        $('#activatechat').show();
    }

    function showInputForm() {
        $('#inputForm').show();
        $('#stopchat').show();
        $('#activatechat').hide();
    }

    function showProtocol()
    {
        $.ajax({
            url: $("#protocollink").val(),
            success: function (data) {
                $('#divprotocol').html(data);
                $('#divrightcont').show();
                isprotocolshowed = true;
            },
            error: function (xhr, status, p3) {
                alert("Ошибка  " + xhr + " " + status + " " + p3);
            }
        });
    }

    function hideProtocol()
    {
        $('#divrightcont').hide();
        isprotocolshowed = false;
    }

    function getProtocolLink()
    {
        $("#protocolhref").attr("href", $("#protdownloadlink").val());
    }

    function resetVote() {
        $(".votestat").empty();
    }

    function showConfirmForm() {
        getProtocolLink();
        $('#confirmForm').show();
        addCommandMessage('Открыто новое голосование.');
    }

});
// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function getProtocolConfirmatinString(status) {
    switch (status[3]) {
        case '1':
            return "Утвердил протокол";
        case '2':
            return "Отклонил протокол";
        case '0':
        default:
            return " ";
    }
}

function Connecting() {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    chat.server.connect($('#userID').val(), $('#chatId').val());
}





