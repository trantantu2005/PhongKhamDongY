"use strict";

window.mediflowUi = window.mediflowUi || {};

window.mediflowUi.scrollTop = function () {
    window.scrollTo({
        top: 0,
        behavior: "smooth"
    });
};

window.mediflowUi.confirmDelete = function (message) {
    return window.confirm(
        message || "Bạn có chắc muốn xóa dữ liệu này?"
    );
};

window.mediflowUi.setPageTitle = function (title) {
    if (!title) {
        return;
    }

    const newTitle = title + " - NATCare";

    if (document.title !== newTitle) {
        document.title = newTitle;
    }
};

window.mediflowUi.translateLoginPage = function () {
    const root = document.querySelector(
        ".natcare-login-page"
    );

    if (!root) {
        return;
    }

    if (document.title !== "Đăng nhập | NATCare") {
        document.title = "Đăng nhập | NATCare";
    }

    const translations = new Map([
        [
            "Enter your user name and password below.",
            "Vui lòng nhập tên đăng nhập và mật khẩu."
        ],
        ["User Name", "Tên đăng nhập"],
        ["Username", "Tên đăng nhập"],
        ["User name", "Tên đăng nhập"],
        ["Password", "Mật khẩu"],
        ["Log In", "Đăng nhập"],
        ["Login", "Đăng nhập"]
    ]);

    const normalizeText = function (value) {
        return (value || "")
            .replace(/\s+/g, " ")
            .trim();
    };

    const setTextIfDifferent = function (
        element,
        newText
    ) {
        if (!element) {
            return;
        }

        const currentText = normalizeText(
            element.textContent
        );

        if (currentText !== newText) {
            element.textContent = newText;
        }
    };

    root.querySelectorAll(
        "label, p, span, " +
        ".dxbl-btn-text, " +
        ".dxbl-text-edit-label"
    ).forEach(function (element) {
        /*
         * Không thay textContent của phần tử chứa thành phần con,
         * tránh làm mất icon hoặc cấu trúc HTML của DevExpress.
         */
        if (
            element.children.length > 0 &&
            !element.classList.contains("dxbl-btn-text")
        ) {
            return;
        }

        const currentText = normalizeText(
            element.textContent
        );

        const translatedText =
            translations.get(currentText);

        if (translatedText) {
            setTextIfDifferent(
                element,
                translatedText
            );
        }
    });

    const passwordInput = root.querySelector(
        'input[type="password"]'
    );

    if (passwordInput) {
        if (
            passwordInput.placeholder !==
            "Nhập mật khẩu"
        ) {
            passwordInput.placeholder =
                "Nhập mật khẩu";
        }

        passwordInput.setAttribute(
            "aria-label",
            "Mật khẩu"
        );

        passwordInput.setAttribute(
            "autocomplete",
            "current-password"
        );
    }

    const userNameInput = Array.from(
        root.querySelectorAll(
            'input:not([type="password"])'
        )
    ).find(function (input) {
        const type = (
            input.type || "text"
        ).toLowerCase();

        return (
            type === "text" ||
            type === "email" ||
            type === "search"
        );
    });

    if (userNameInput) {
        if (
            userNameInput.placeholder !==
            "Nhập tên đăng nhập"
        ) {
            userNameInput.placeholder =
                "Nhập tên đăng nhập";
        }

        userNameInput.setAttribute(
            "aria-label",
            "Tên đăng nhập"
        );

        userNameInput.setAttribute(
            "autocomplete",
            "username"
        );
    }

    const submitButton = root.querySelector(
        'button[type="submit"], ' +
        '.logon-toolbar .btn-primary'
    );

    if (submitButton) {
        const textContainer =
            submitButton.querySelector(
                ".dxbl-btn-text"
            );

        if (textContainer) {
            setTextIfDifferent(
                textContainer,
                "Đăng nhập"
            );
        } else if (
            submitButton.children.length === 0
        ) {
            setTextIfDifferent(
                submitButton,
                "Đăng nhập"
            );
        }
    }
};

(function initializeMediFlowUi() {
    let translationScheduled = false;

    const runTranslation = function () {
        translationScheduled = false;

        try {
            window.mediflowUi.translateLoginPage();
        } catch (error) {
            console.error(
                "Không thể Việt hóa trang đăng nhập:",
                error
            );
        }
    };

    const scheduleTranslation = function () {
        /*
         * Chỉ cho phép một lần xử lý trong mỗi frame,
         * tránh observer chạy liên tục.
         */
        if (translationScheduled) {
            return;
        }

        translationScheduled = true;

        window.requestAnimationFrame(
            runTranslation
        );
    };

    const startObserver = function () {
        scheduleTranslation();

        if (!document.body) {
            return;
        }

        const observer = new MutationObserver(
            function () {
                scheduleTranslation();
            }
        );

        observer.observe(
            document.body,
            {
                childList: true,
                subtree: true
            }
        );
    };

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            startObserver,
            {
                once: true
            }
        );
    } else {
        startObserver();
    }
})();