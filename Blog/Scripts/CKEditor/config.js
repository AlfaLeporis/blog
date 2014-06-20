/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
    config.extraPlugins = 'wpmore,dialog,dialogui,syntaxhighlight';
    config.toolbar = [['Save', '-', 'Source', 'Preview', '-', 'Templates'],
                ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Print', 'SpellChecker', 'Scayt'],
                ['Undo', 'Redo', '-', 'Find', 'Replace'],
                ['Link', 'Unlink', 'Anchor'],
                '/',
                ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv'],
                ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
                ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
                ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak'],
                 '/',
                ['Styles', 'Format', 'Font', 'FontSize'],
                ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
                ['Maximize'],
                ['Hello', 'Hello2'],
                ['Source', 'WPMore'],
                ['Syntaxhighlight']];
    config.height = "500px";
    config.filebrowserBrowseUrl = '/Administrator/Editor/FilesBrowser';
    config.filebrowserUploadUrl = '/Administrator/Editor/Uploader';
};
