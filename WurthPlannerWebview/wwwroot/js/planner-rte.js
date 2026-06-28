window.plannerRte = {
    init: function (editorId, initialHtml) {
        const el = document.getElementById(editorId);
        if (!el) return;
        el.innerHTML = initialHtml || '';
    },

    getContent: function (editorId) {
        const el = document.getElementById(editorId);
        return el ? el.innerHTML : '';
    },

    setContent: function (editorId, html) {
        const el = document.getElementById(editorId);
        if (!el) return;
        if (el.innerHTML !== html) {
            el.innerHTML = html || '';
        }
    },

    exec: function (editorId, command, value) {
        const el = document.getElementById(editorId);
        if (!el) return;
        el.focus();
        document.execCommand(command, false, value || null);
    },

    promptForLink: function () {
        return window.prompt('URL du lien :', 'https://');
    }
};
