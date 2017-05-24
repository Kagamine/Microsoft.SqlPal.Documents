var anchors = $('.doc-nav-list a');
for (var i = 0; i < anchors.length; i++) {
    var nxt = $(anchors[i]).next();
    if (nxt.length > 0 && nxt[0].tagName.toUpperCase() == 'UL') {
        if ($(anchors[i]).attr('href').indexOf('javascript:') >= 0)
            $(anchors[i]).prepend('<i class="fa fa-angle-right"></i>');
        else
            $(anchors[i]).prepend('<i class="fa fa-angle-right" onclick="Expand(this)"></i>');
    } else {
        $(anchors[i]).addClass('non-arrow');
    }
    if ($(anchors[i]).attr('href').indexOf('http') < 0 && $(anchors[i]).attr('href').indexOf('//') < 0 && $(anchors[i]).attr('href').indexOf('javascript') < 0 && $(anchors[i]).attr('href').indexOf('#') < 0)
        $(anchors[i]).attr('href', '/' + $(anchors[i]).attr('href'));
    else if ($(anchors[i]).attr('href').indexOf('http') >= 0 && $(anchors[i]).attr('href').indexOf('//') >= 0)
        $(anchors[i]).attr('target', '_blank');
}

if (anchors.length > 1) {
    $(anchors[0]).children('i').remove();
}

var active = $('a[href="' + endpoint + '"]');
$('head').append('<title>' + active.text() + '</title>');
active.children('i').removeClass('fa-angle-right').addClass('fa-angle-down');
active.addClass('active');
var current = active;
while (current.length > 0) {
    current.parents('ul').show();
    current.prev().children('i').removeClass('fa-angle-right').addClass('fa-angle-down');
    current.next().show();
    current = current.parents('ul');
}

var blocks = $('blockquote');
for (var i = 0; i < blocks.length; i++) {
    $(blocks[i]).html($(blocks[i]).html().replace(/\n/g, "</p>\n<p>"));
    if ($(blocks[i]).html().indexOf('[!TIP]') >= 0) {
        $(blocks[i]).html($(blocks[i]).html().split('[!TIP]').join('<strong>TIP</strong>'));
        $(blocks[i]).addClass('tip');
    }
    if ($(blocks[i]).html().indexOf('[!NOTE]') >= 0) {
        $(blocks[i]).html($(blocks[i]).html().split('[!NOTE]').join('<strong>NOTE</strong>'));
        $(blocks[i]).addClass('info');
    }
    if ($(blocks[i]).html().indexOf('[!WARNING]') >= 0) {
        $(blocks[i]).html($(blocks[i]).html().split('[!WARNING]').join('<strong>WARNING</strong>'));
        $(blocks[i]).addClass('warn');
    }
}

function Expand(anchor) {
    var ul, a;
    if ($(anchor).is('a'))
    {
        a = $(anchor);
        ul = $(anchor).next();
    }
    else if ($(anchor).is('i'))
    {
        a = $(anchor).parents('a');
        ul = $(anchor).parents('a').next();
    }
    ul.slideToggle();
    if (a.children('i').hasClass('fa-angle-right'))
        a.children('i').removeClass('fa-angle-right').addClass('fa-angle-down');
    else
        a.children('i').removeClass('fa-angle-down').addClass('fa-angle-right');
}

$(document).click(function (e) {
    if ($(e.target).is('i'))
    {
        e.preventDefault();
    }
});

$('.doc-nav-list').scrollTop(active.offset().top + $('.doc-nav-list').scrollTop() - 115);
Highlight();