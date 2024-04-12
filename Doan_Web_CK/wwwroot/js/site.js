function createTableRow(item) {

    var row = document.createElement("tr");

    var idCell = document.createElement("td");
    idCell.textContent = item.Id;
    row.appendChild(idCell);

    var titleCell = document.createElement("td");
    titleCell.textContent = item.Title;
    row.appendChild(titleCell);

    var accountIdCell = document.createElement("td");
    accountIdCell.textContent = item.AccountId;
    row.appendChild(accountIdCell);

    var isAcceptedCell = document.createElement("td");
    isAcceptedCell.textContent = item.IsAccepted;
    row.appendChild(isAcceptedCell);

    var publishDateCell = document.createElement("td");
    publishDateCell.textContent = item.PublishDate;
    row.appendChild(publishDateCell);

    var likesCell = document.createElement("td");
    likesCell.textContent = item.Likes;
    row.appendChild(likesCell);

    var actionsCell = document.createElement("td");
    actionsCell.classList.add("d-flex", "flex-column", "gap-2");

    // Tạo button Accept hoặc UnAccept tùy thuộc vào giá trị của item.IsAccepted
    var acceptForm = document.createElement("form");
    acceptForm.action = item.IsAccepted ? "/Admin/BlogAdmin/UnAccept" : "/Admin/BlogAdmin/Accept";
    acceptForm.method = "post";
    var acceptButton = document.createElement("button");
    acceptButton.type = "submit";
    acceptButton.classList.add("w-100", "btn", "btn-outline-light");
    acceptButton.textContent = item.IsAccepted ? "UnAccept" : "Accept";
    acceptForm.appendChild(acceptButton);
    actionsCell.appendChild(acceptForm);

    // Tạo button Details
    var detailsLink = document.createElement("a");
    detailsLink.href = "/Admin/BlogAdmin/Details/" + item.Id;
    detailsLink.classList.add("btn", "btn-outline-light");
    detailsLink.textContent = "Details";
    actionsCell.appendChild(detailsLink);

    // Tạo button Delete và Modal
    var deleteButton = document.createElement("button");
    deleteButton.type = "button";
    deleteButton.classList.add("btn", "btn-outline-light");
    deleteButton.dataset.bsToggle = "modal";
    deleteButton.dataset.bsTarget = "#exampleModal";
    deleteButton.textContent = "Delete";
    actionsCell.appendChild(deleteButton);

    // Thêm cell chứa các button vào row
    row.appendChild(actionsCell);

    // Trả về row đã tạo
    return row;
}
var tbody = document.getElementById('tbody');
function handleSearchBlog(event) {
    let search = event.target.value
    fetch(`/Admin/BlogAdmin/GetAllBlogAsync?search=${search}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log(data)
            if (tbody) {
                tbody.innerHTML = '';
                data.blogs.forEach(item => {
                    tbody.appendChild(createTableRow(item));
                });
            }

        })
        .catch(error => {
            console.log('There was a problem with the fetch operation:', error);
        });
}

function handleAddFriendProfile(userId, friendId) {

    fetch(`/Profile/AddFriend?form_add_friend_userid=${userId}&form_add_friend_friendid=${friendId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            //console.log(data);
            var element = document.getElementById("profile_request_friend")
            if (element) {
                element.innerHTML = ''
                element.innerHTML = data.newHtml
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}


var messages_display_container = document.getElementById("messages_display_container")
if (messages_display_container) {
    messages_display_container.scrollTop = messages_display_container.scrollHeight;
}

$(document).ready(function () {
    var screenWidth = $(window).width();
    var screenHeight = $(window).height();
    //console.log("Check width" + screenWidth)

    var ele = document.getElementById('nof_icon_container');

    if (screenWidth > 550) {
        ele.innerHTML = `
                <a onclick="handleToggleNofs()" id="navNofitication" class="nof_icon text-center">
                    <i class="navIcon icon bi bi-bell-fill"></i>
                    <p class="navTitle w-75 fs-4 text-start my-auto">Nofitications</p>
                </a>
            `
    }
});

function handleDeleteBlog(blogId) {
    fetch(`/Blog/Delete?blogId=${blogId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            //console.log(data);
            var port = window.location.port;
            if (window.location.href.indexOf("Profile") !== -1) { // Sử dụng indexOf() để kiểm tra chuỗi con
                window.location.href = "https://localhost:" + port + "/Profile";
            } else {
                window.location.href = "https://localhost:" + port;
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function handleDeleteComment(commentId, blogId) {
    fetch(`/Blog/DeleteComment?commentId=${commentId}&blogId=${blogId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            //console.log(data);
            //console.log(blogId);
            // cập nhật giao diện người dùng với dữ liệu mới
            var commentContainer = document.getElementById("comment_container_lower_" + blogId);
            if (data.message == 'success') {
                showElement("update_success")
                commentContainer.innerHTML = ''
                commentContainer.innerHTML = data.newHtml
            } else {
                showElement("update_fail")
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function handleDeleteNofitication(nofId) {
    fetch(`/Blog/DeleteNofitication?nofId=${nofId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data);
            var element = document.getElementById("nav_item_nofitications_container")
            if (element) {
                element.innerHTML = ''
                element.innerHTML = data.newHtml
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function reloadAllNof(userId) {
    fetch(`/Profile/GetReloadNofs?userId=${userId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data.newHtml);
            // cập nhật giao diện người dùng với dữ liệu mới
            var element = document.getElementById("nav_item_nofitications_container")
            if (element) {
                element.innerHTML = '';
                element.innerHTML = data.newHtml
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function handleToggleNofs() {
    var element = document.getElementById("nav_item_nofitications");
    if (element.classList.contains("hidden")) {
        element.classList.remove('hidden');
    } else {
        element.classList.add("hidden");
    }
}

function editblog() {
    event.preventDefault();
    var formData = new FormData(document.getElementById("edit_form"));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/Edit", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                    if (response.message == 'failed') {

                        showElement("update_fail");
                    } else {

                        showElement("update_success");
                        // Lấy thông tin về cổng của localhost hiện tại
                        var port = window.location.port;

                        // Chuyển hướng trang đến localhost với cổng hiện tại
                        window.location.href = "https://localhost:" + port;
                    }
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}

function handleAccept(userId, nofId) {
    fetch(`/Profile/AcceptFriendRequest?userId=${userId}&nofId=${nofId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data.commentHtml);
            // cập nhật giao diện người dùng với dữ liệu mới
            var element = document.getElementById("nofi_card_actions_" + nofId);
            if (element) {
                element.innerHTML = '';
                element.innerHTML = data.newHtml;
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function UpdateProfile() {
    event.preventDefault();
    var formData = new FormData(document.getElementById("update_profile_form"));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Profile/UpdateProfile", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                    if (response.message == 'success') {
                        console.log("vao success")
                        showElement("update_success");
                    } else {
                        console.log("vao failed")
                        showElement("update_fail");
                    }
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function showElement(elementId) {
    // Lấy element
    const element = document.getElementById(elementId);

    // Hiển thị element
    element.style.display = "block";

    // Ẩn element sau 5 giây
    setTimeout(() => {
        element.style.display = "none";
    }, 3000);
}
function handleDeny(userId, nofId) {
    fetch(`/Profile/DenyFriendRequest?userId=${userId}&nofId=${nofId}`,)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data.commentHtml);
            // cập nhật giao diện người dùng với dữ liệu mới
            var element = document.getElementById("nofi_card_actions_" + nofId);
            if (element) {
                element.innerHTML = '';
                element.innerHTML = data.newHtml;
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function handleUnFriend(userId, friendId, itemId) {
    fetch(`/Friend/UnFriend?userId=${userId}&friendId=${friendId}`,)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data);
            var elementFriendIndex = document.getElementById("friend_card_actions_" + itemId)
            var elementProfileIndex = document.getElementById("profile_request_friend")
            var elementBLogIndex = document.getElementById("right_click_menu_container_" + itemId)
            if (elementBLogIndex) {
                elementBLogIndex.innerHTML = ''
                elementBLogIndex.innerHTML = data.sbBlogIndex
            }
            if (elementProfileIndex) {
                elementProfileIndex.innerHTML = ''
                elementProfileIndex.innerHTML = data.sbProfile
            }
            if (elementFriendIndex) {
                elementFriendIndex.innerHTML = ''
                elementFriendIndex.innerHTML = data.sbFriendIndex
                showElement("friend_card_status_" + itemId)
            }


        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function handleAddFriend(userId, blogId) {
    console.log(userId.trim());
    console.log(blogId);

    var formData = new FormData(document.getElementById("form_add_friend_" + blogId));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/AddFriend", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                    var element = document.getElementById("right_click_menu_container_" + blogId)
                    if (element) {
                        element.innerHTML = '';
                        element.innerHTML = response.newHtml;
                    }
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function handleRightClick(id) {
    event.preventDefault();
    var element = document.getElementById("right_click_menu_" + id);
    if (element.classList.contains("hidden")) {
        element.classList.remove("hidden")
    } else {
        element.classList.add("hidden")
    }
}
function handleShareBlog(id) {
    event.preventDefault();
    var formData = new FormData(document.getElementById("share_form_" + id));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/ShareBlog", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                    showElement("share_status_success");
                } else {
                    showElement("share_status_failed")
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function handleToggleShareActions(id) {
    var element = document.getElementById("share_actions_" + id)
    if (element.classList.contains("hidden")) {
        element.classList.remove("hidden");
    } else {
        element.classList.add("hidden")
    }
}
function handleToggleLike(id) {
    event.preventDefault();
    var formData = new FormData(document.getElementById("like_form_" + id));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/AddLike", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
let isDisplayAllCmt = true
function handleLoadAllComments(blogId) {
    console.log("Typeof isDisplayAllCmt >>> ", typeof (isDisplayAllCmt))
    fetch(`/Blog/GetAllCommentsOfBlogs?blogId=${blogId}&isDisplayAllCmt=${isDisplayAllCmt}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // xử lý dữ liệu trả về
            console.log(data.commentHtml);
            // cập nhật giao diện người dùng với dữ liệu mới
            var commentContainer = document.getElementById("comment_container_lower_" + blogId);
            if (commentContainer) {
                commentContainer.innerHTML = '';
                commentContainer.innerHTML = data.commentHtml;
                isDisplayAllCmt = !isDisplayAllCmt;
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}
function handleChangeIcon(id) {
    var icon = document.getElementById("like_icon_" + id);
    if (!icon.classList.contains("bi-heart-fill")) {
        icon.classList.remove("bi-heart")
        icon.classList.add("bi-heart-fill")
    } else {
        icon.classList.remove("bi-heart-fill")
        icon.classList.add("bi-heart")
    }
}
function handleToggleUnLike(id) {
    event.preventDefault();
    var formData = new FormData(document.getElementById("like_form_" + id));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/UnLike", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response) {
                    console.log("CHECK response >>> ", response)
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function handleUnLike(id) {
    handleChangeIcon(id)

    let element = document.getElementById("action_like_" + id)
    console.log(element);
    element.onclick = function () {
        handleLike(id)
    }
    console.log(element);

    handleToggleUnLike(id)
}
function handleLike(id) {
    handleChangeIcon(id)

    let element = document.getElementById("action_like_" + id)
    console.log(element);
    element.onclick = function () {
        handleUnLike(id)
    }
    console.log(element);
    handleToggleLike(id)
}

if (document.querySelector("#profile_photo_input")) {
    document.querySelector("#profile_photo_input").addEventListener("change", handlePreviewPhoto);
    if (!document.getElementById("profile_photo_input").value != "") {
        console.log("Chay lan dau")
        document.getElementById("saveChange_button").classList.remove("flex")
        document.getElementById("saveChange_button").classList.add("hidden")
    }
}

function handlePreviewPhoto() {
    console.log("Da chay")
    if (!this.files || !this.files[0]) return;

    const FR = new FileReader();
    var img = document.getElementById('profile_photo');

    FR.addEventListener("load", function (evt) {
        img.src = evt.target.result;
    });

    console.log("Check >>> ", img.src)

    FR.readAsDataURL(this.files[0]);

    if (document.getElementById("profile_photo_input").value != "") {
        console.log("Chay lan hai")
        document.getElementById("saveChange_button").classList.remove("hidden")
        document.getElementById("saveChange_button").classList.add("flex")
    } else {
        console.log("Chay lan dau")
        document.getElementById("saveChange_button").classList.remove("flex")
        document.getElementById("saveChange_button").classList.add("hidden")
    }
}
function syncInputs(value, targetId) {
    var targetInput = document.getElementById(targetId);
    targetInput.value = value;
    console.log("dang chay syncinput")
}

function handleEdituserName() {
    var input_username = document.getElementById("input_username_edit")
    if (!input_username.classList.contains("hidden")) {
        document.getElementById("saveChange_button").classList.remove("hidden")
        document.getElementById("saveChange_button").classList.add("flex")
    } else {
        document.getElementById("saveChange_button").classList.remove("flex")
        document.getElementById("saveChange_button").classList.add("hidden")
    }
    console.log("dang chay handleEdituserName")
}
function toogleEditUsername() {
    var h1 = document.getElementById("user_name_h1");
    var input_username = document.getElementById("input_username_edit")
    if (!h1.classList.contains("hidden")) {
        h1.classList.add("hidden")
        input_username.classList.remove("hidden")
    }
    document.getElementById("saveChange_button").classList.remove("hidden")
    document.getElementById("saveChange_button").classList.add("flex")

}
function handleEditToggleComment(id) {

    var form = document.getElementById("comment_form_" + id)
    var p = document.getElementById("p_content_" + id);
    if (!p.classList.contains("hidden")) {
        p.classList.add("hidden");
        form.classList.remove("hidden");
        form.focus();
    } else {
        p.classList.remove("hidden")
        form.classList.add("hidden")
    }
}
function toggleActionComment(id) {
    var element = document.getElementById("comments_actions_" + id)
    if (element.classList.contains("hidden")) {
        element.classList.remove("hidden");
    } else {
        element.classList.add("hidden");
    }
}
function handleSubmitAddCmt(id) {
    event.preventDefault();
    var formData = new FormData(document.getElementById("form_comment_" + id));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/AddComment", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);

                // Cập nhật nội dung của phần tử commentContainer
                document.getElementById("form_input_comment_" + id).value = ""
                var commentContainer = document.getElementById("comment_container_lower_" + id);
                if (commentContainer) {
                    commentContainer.innerHTML = '';
                    commentContainer.innerHTML = response.commentHtml;
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function handleToggleMore(id) {
    //console.log(id)
    const element = document.getElementById("content_" + id);
    const toggle_icon = document.getElementById("toggle_icon_" + id)
    var parentElement = element.parentElement
    if (element.classList.contains("temp_hidden")) {

        element.classList.remove("temp_hidden")
        toggle_icon.classList.remove("bi-arrow-down-circle")
        toggle_icon.classList.add("bi-arrow-up-circle")

    } else {
        parentElement.scrollIntoView({ behavior: 'smooth' })
        //console.log(parentElement)
        element.classList.add("temp_hidden")
        // scroll execute
        toggle_icon.classList.remove("bi-arrow-up-circle")
        toggle_icon.classList.add("bi-arrow-down-circle")
    }
}

function handleToggle(id) {
    var element = document.getElementById(id);
    if (element.classList.contains("hidden")) {
        element.classList.remove("hidden");
        element.classList.add("flex");
    } else {
        element.classList.add("hidden");
        element.classList.remove("flex");
    }
}
function handleSubmitEditCmt(id, cmtId) {
    event.preventDefault();
    //console.log("Check cmtid >>> ", cmtId)
    var formData = new FormData(document.getElementById("edit_form_cmt_" + cmtId));
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Blog/UpdateComment", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest"); // Đảm bảo yêu cầu được nhận biết là AJAX
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);

                var commentContainer = document.getElementById("comment_container_lower_" + id);
                if (commentContainer) {
                    commentContainer.innerHTML = '';
                    commentContainer.innerHTML = response.commentHtml;
                }
            } else {
                // Xử lý lỗi
            }
        }
    };
    xhr.send(formData);
}
function handleKeyPressInputEditComment(event, id, commentId) {
    if (event.key === 'Enter') {
        handleSubmitEditCmt(id, commentId)

        console.log("Check" + commentId);
        handleEditToggleComment(commentId)
    }
}

function handleKeyPress(event, id) {
    // Kiểm tra xem phím được nhấn có phải là phím Enter không
    if (event.key === "Enter") {
        // Xử lý sự kiện tại đây, ví dụ: gọi một hàm JavaScript hoặc thực hiện một hành động khác
        handleSubmitAddCmt(id)
    }
    return false;
}

function handleToggleFilter() {

    var element = document.getElementById("filter_modal");
    if (element) {
        if (element.classList.contains("hidden")) {
            element.classList.remove("hidden");
            element.classList.add("block");
        } else {
            element.classList.add("hidden");
            element.classList.remove("block");
        }
    }
}