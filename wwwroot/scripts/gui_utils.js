// Copyright (c) 2018 Joeytje50. All rights reserved.
// This code is licenced under GNU GPL3+; see <http://www.gnu.org/licenses/>

// Source code and most recent version:
// https://gist.github.com/Joeytje50/2b73f9ac47010e7fdc5589788b80af77

// select all <options> within `sel` in the range [from,to]
// change their state to the state sel.options[from] is in,
// ie. change it to the current state of the first selected element
function selectRange(sel, from, to) {
    var toState = sel.options[from].selected;
    // make sure from < to:
    if (from > to) {
        var temp = from;
        from = to;
        to = temp;
    }
    if (!(sel instanceof HTMLSelectElement)) {
        throw new TypeError("selectRange requires a single non-jQuery select-element as first parameter");
    }
    // (de)select every element
    for (var i = from; i <= to; i++) {
        sel.options[i].selected = toState;
    }
}

$(function () {
    $("#results").data("select-start", 0); // default selection start
    $("#results").on("mousedown", ".addSelect", function (e) {
        $(this).focus();
        // clicking on the edge of the <select> shouldn't do anything special
        if (!$(e.target).is("option")) return;
        // if Ctrl is pressed, just let the built-in functionality take over
        if (e.ctrlKey) return;
        // keep everything selected that's not affected by the range within a shift-click
        if (e.shiftKey) {
            var fromIdx = $("#results").data("select-start");
            selectRange(this, $("#results").data("select-start"), e.target.index);
            e.preventDefault();
            return false;
        }
        // save the starting <option> and the state to change to
        $("#results").data("select-start", e.target.index);
        e.target.selected = !e.target.selected;
        e.preventDefault();
        // save a list of selected elements, to make sure only the selected <options>
        // are added or removed when dragging
        var selected = [];
        for (var i = 0; i < this.selectedOptions.length; i++) {
            selected.push(this.selectedOptions[i].index);
        }
        $(this).data("selected", selected);
        $(this).children("option").on("mouseenter", function (e) {
            var sel = this.parentElement;
            // first reset all options to the original state
            for (var i = 0; i < sel.options.length; i++) {
                if ($(sel).data("selected").indexOf(i) == -1) {
                    sel.options[i].selected = false;
                } else {
                    sel.options[i].selected = true;
                }
            }
            // then apply the new range to the elements
            selectRange(sel, $("#results").data("select-start"), e.target.index);
        });
		
		e.target.selected = !e.target.selected;
		e.target.selected = !e.target.selected;
		
		console.log($(this).attr('id'))
		console.log($(e.target).attr('id'))
		
		//var optionTop = $(e.target).offset().top;
		//var selectTop = $(this).offset().top;
		//$(this).scrollTop($(this).scrollTop() + (optionTop - selectTop));
    });
    // clean up events after click event has ended.
    $(window).on("mouseup", function () {
        $(".addSelect").children("option").off("mouseenter"); // remove mouseenter-events
    });
});

// fix for charisma scripts on ajax reload
$(document).on('click', '.btn-minimize', function(e){
	e.preventDefault();
	var $target = $(this).parent().parent().next('.box-content');
	if ($target.is(':visible')) $('i', $(this)).removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
	else                       $('i', $(this)).removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
	$target.slideToggle();
});