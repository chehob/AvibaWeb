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
	
	$(document).on('click', '.btn-setting', function (e) {
        e.preventDefault();
        $('#dialog').modal('show');
    });
	
	$(document).on('click', '.accountLink', function (e) {
        e.preventDefault();
        $.ajax({
			url: "/Report/AccountOperations",
			type: "GET",
			cache: false,
			data: {
				accountId: $(this).attr("data-account-id")
			},
			success: function (getResult) {
				$("#results").html(getResult);
			}
		});
    });
});

// fix for charisma scripts on ajax reload
$(document).on('click', '.btn-minimize', function (e) {
    e.preventDefault();
    var $target = $(this).parent().parent().next('.box-content');
    if ($target.is(':visible')) $('i', $(this)).removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
    else $('i', $(this)).removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
    $target.slideToggle();
});

$(document).on('click', '.hdr-minimize', function (e) {
    e.preventDefault();
    var $target = $(this).next('.box-content');
    if ($target.is(':visible')) $('i', $(this)).removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
    else $('i', $(this)).removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
    $target.slideToggle();
});

function updateBalance(ajaxContext) {
    const $div = $("#dvUserBalance");
    $div.load($div.data("url"));
}

function loadBookingManagementFilter(ajaxContent) {
    const $div = $("#dvFilter");
    $div.load($div.data("url"));
}

function numberWithSpaces(x) {
	var parts = x.toString().split(".");
	parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ");
	return parts.join(".");
}

// ManagementAccounting Summary
$(document).on('click', '#summaryOrganizationBalanceDiv', function (e) {
	const block = document.getElementById("cashlessBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashBlock,#transitBlock,#corporatorsBlock,#providersBlock,#subagentsBlock").each(function(){
		this.classList.add("hidden");
	});
	
});
$(document).on('click', '#CashTotalStr', function (e) {
	const block = document.getElementById("cashBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashlessBlock,#transitBlock,#corporatorsBlock,#providersBlock,#subagentsBlock").each(function(){
		this.classList.add("hidden");
	});
	
});
$(document).on('click', '#CorpNegativeBalanceStr', function (e) {
	const block = document.getElementById("corporatorsBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashlessBlock,#transitBlock,#cashBlock,#providersBlock,#subagentsBlock").each(function(){
		this.classList.add("hidden");
	});
	
});
$(document).on('click', '#TransitTotalStr', function (e) {
	const block = document.getElementById("transitBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashlessBlock,#corporatorsBlock,#cashBlock,#providersBlock,#subagentsBlock").each(function(){
		this.classList.add("hidden");
	});
	
});
$(document).on('click', '#ProvidersTotalStr', function (e) {
	const block = document.getElementById("providersBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashlessBlock,#corporatorsBlock,#cashBlock,#transitBlock,#subagentsBlock").each(function(){
		this.classList.add("hidden");
	});
	
});
$(document).on('click', '#SubagentsTotalStr', function (e) {
	const block = document.getElementById("subagentsBlock");
	if (block.classList.contains("hidden")) {
		block.classList.remove("hidden");
	} else {
		block.classList.add("hidden");
	}

	$("#cashlessBlock,#corporatorsBlock,#cashBlock,#transitBlock,#providersBlock").each(function(){
		this.classList.add("hidden");
	});
	
});

$(document).on('click', '#summaryCashlessTotalDiv', function (e) {
	e.preventDefault();
	$.ajax({
		url: "/Report/CashlessOrg",
		type: "GET",
		cache: false,
		success: function(result) {
			$("#contentResults").html(result);
		},
		error: function(error) {
			console.log(error);
		}
	});
});

$(document).on('click',
        '.clearReceipt',
        function (e) {
            var button = $(this);
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ClearReceipt",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    button.parent().siblings()[2].innerHTML = "";
                    button.parent().siblings()[3].innerHTML = "";
                    button.parent().siblings()[4].innerHTML = `<span>0.00</span>`;
                    button.parent().siblings()[5].innerHTML = `<span class="label-default label label-danger">Не оплачен</span>`;
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

$(document).on('click',
        '.createReceiptPDF',
        function (e) {

            var attr = $(this).attr('data-signature');

        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];
                var itemCount = 0;
                result.items.forEach(function(item) {
                    dataRow = [];

                    itemCount++;
                    dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                    dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                    dataRow.push({ text: item.segCount, alignment: 'center' });
                    dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });

                    bodyData.push(dataRow);
                });

                result.luggageItems.forEach(function(item) {
                    dataRow = [];

                    itemCount++;
                    dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                    dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                    dataRow.push({ text: item.segCount, alignment: 'center' });
                    dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });

                    bodyData.push(dataRow);
                });

                if(result.items.length == 0 && result.luggageItems.length == 0) {
                    dataRow = [];

                    itemCount++;
                    dataRow.push({ text: '1', alignment: 'center' });
                    dataRow.push({ text: 'Предоплата за авиа/жд билеты', style: 'smallText' });
                    dataRow.push({ text: '1', alignment: 'center' });
                    dataRow.push({ text: '', alignment: 'center' });
                    dataRow.push({ text: result.totalAmountStr, alignment: 'right' });
                    dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                    bodyData.push(dataRow);
                }

                result.taxes.forEach(function(item) {
                    dataRow = [];

                    itemCount++;
                    dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                    dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                    dataRow.push({ text: item.segCount, alignment: 'center' });
                    dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                    dataRow.push({ text: item.feeStr, alignment: 'right' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });

                    bodyData.push(dataRow);
                });

                // itemCount++;
                // dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                // dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                // dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                // dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                // dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                // dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                // bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    info: {
                        title: `Счет №${result.receiptNumber}`
                      },
                    content: [
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: ${itemCount}, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText',
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { 
                                            text: 'Руководитель',
                                            margin: [0, 40, 0, 0]
                                        },
                                        (typeof attr == typeof undefined || attr == false || attr == 'true') ? 
                                            { 
                                                image: `${result.signatureImage}`,
                                                width: 150,
                                                alignment: 'center',
                                                margin: [25, 0, 25, 0]
                                            } : '',                                                                        
                                        (typeof attr == typeof undefined || attr == false || attr == 'true') ?
                                            {
                                                image: `${result.stampImage}`,
                                                width: 125,
                                                margin: [70, 50, 0, 0]
                                            } : '',
                                        { 
                                            text: `${result.orgHeadName}`,
                                            margin: [-100, 40, 0, 0]
                                        }                                      
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center'
                        },
                        mediumText: {
                            fontSize: 10,
                        },
                        smallText: {
                            fontSize: 8
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).open();
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
    '.ticketCancelConfirmBtn[data-toggle=confirmation]',
    function (e) {
        e.preventDefault();
        $(this).confirmation({
            rootSelector: '[data-toggle=confirmation]',
            onConfirm: function (e) {
                var button = $(this);

                $.ajax({
                    url: "/Management/CancelTicket",
                    type: "POST",
                    cache: false,
                    data: {
                        id: $(this).closest('tr').find('.ticketId').val(),
                        cancelOpType: $("#tcoAccepted").val()
                    },
                    success: function (result) {
                        $("#getTicketListBtn").trigger("click");
                    },
                    error: function (error) {
                        console.log(error.message);
                    }
                });
            }
        });
        $(this).confirmation('show');
    });

    $(document).on('click',
        '.createReceiptPDF2',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];

                dataRow.push({ text: '1', alignment: 'center' });
                dataRow.push({ text: 'Оплата за авиабилеты', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({});
                dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: '2', alignment: 'center' });
                dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    content: [
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: 2, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText'
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { text: 'Руководитель', margin: [0, 40, 0, 0]},
                                        { image: `${result.signatureImage}`, width: 150, alignment: 'center', margin: [25, 0, 25, 0] },
                                        { text: `${result.orgHeadName}`, margin: [0, 40, 0, 0] },
                                        { image: `${result.stampImage}`, width: 125, alignment: 'center', margin: [25, 0, 45, 0] }
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center'
                        },
                        mediumText: {
                            fontSize: 10,
                        },
                        smallText: {
                            fontSize: 8
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).open();
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
        '.createReceiptPDF3',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];

                dataRow.push({ text: '1', alignment: 'center' });
                dataRow.push({ text: 'Предоплата за авиабилеты', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({});
                dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: '2', alignment: 'center' });
                dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    content: [
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: 2, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText'
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { text: 'Руководитель', margin: [0, 40, 0, 0]},
                                        { image: `${result.signatureImage}`, width: 150, alignment: 'center', margin: [25, 0, 25, 0] },
                                        { text: `${result.orgHeadName}`, margin: [0, 40, 0, 0] },
                                        { image: `${result.stampImage}`, width: 125, alignment: 'center', margin: [25, 0, 45, 0] }
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center'
                        },
                        mediumText: {
                            fontSize: 10,
                        },
                        smallText: {
                            fontSize: 8
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).open();
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
        '.createAvibaReceiptPDF',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];

                dataRow.push({ text: '1', alignment: 'center' });
                dataRow.push({ text: 'Предоплата за авиабилеты', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({});
                dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: '2', alignment: 'center' });
                dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    content: [
                        { 
                            image: `${result.headerImage}`,
                            margin: [-40, -40, 0, 0],
                            width: 595.28
                        },
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: 2, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText'
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { text: 'Руководитель', margin: [0, 40, 0, 0]},
                                        { image: `${result.signatureImage}`, width: 150, alignment: 'center', margin: [25, 0, 25, 0] },
                                        { text: `${result.orgHeadName}`, margin: [0, 40, 0, 0] },
                                        { image: `${result.stampImage}`, width: 125, alignment: 'center', margin: [25, 0, 45, 0] }
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center',
                            color: '#012659'
                        },
                        mediumText: {
                            fontSize: 10,
                            color: '#012659'
                        },
                        smallText: {
                            fontSize: 8,
                            color: '#012659'
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center',
                            color: '#012659'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).open();
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
        '.createSiteReceiptPDF',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];

                dataRow.push({ text: '1', alignment: 'center' });
                dataRow.push({ text: 'Предоплата за авиабилеты', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({});
                dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: '2', alignment: 'center' });
                dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    content: [
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: 2, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText'
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { text: 'Руководитель', margin: [0, 40, 0, 0]},
                                        { image: `${result.signatureImage}`, width: 150, alignment: 'center', margin: [25, 0, 25, 0] },
                                        { text: `${result.orgHeadName}`, margin: [0, 40, 0, 0] },
                                        { image: `${result.stampImage}`, width: 125, alignment: 'center', margin: [25, 0, 45, 0] }
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center'
                        },
                        mediumText: {
                            fontSize: 10
                        },
                        smallText: {
                            fontSize: 8
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).open();
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
        '.downloadReceiptPDF3',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $(this).closest('tr').find('.receiptId').val() },
            success: function (result) {
                console.log(result);
                const bodyData = [];

                const headerRow = [];

                headerRow.push({ text: '№', style: 'tableHeader' });
                headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                headerRow.push({ text: 'Цена', style: 'tableHeader' });
                headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                bodyData.push(headerRow);

                var dataRow = [];

                dataRow.push({ text: '1', alignment: 'center' });
                dataRow.push({ text: 'Предоплата за авиабилеты', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({});
                dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: '2', alignment: 'center' });
                dataRow.push({ text: 'Сбор за оформление авиабилета', style: 'smallText' });
                dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                bodyData.push(dataRow);

                dataRow = [];

                dataRow.push({ text: 'Итого: ', colSpan: 5, alignment: 'right' });
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({});
                dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                bodyData.push(dataRow);

                const docDefinition = {
                    content: [
                        {
                            text: 'Внимание! Счет действителен для оплаты в течение 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
                            style: 'headerWarning'
                        },
                        {
                            text: result.orgAccountWarningStr,
                            style: 'headerWarning',
                            color: '#ff0000',
                            margin: [0, 10, 0, 0]
                        },
                        {
                            table: {
                                widths: [150, 150, 40, '*'],
                                heights: [15, 30, 15, 15],

                                body: [
                                    [`ИНН: ${result.orgITN}`, `КПП: ${result.orgKPP}`, { text: 'Сч. №', rowSpan: 2, margin: [0, 35, 0, 0] }, { text: result.orgFinancialAccount, rowSpan: 2, margin: [0, 35, 0, 0] }],
                                    [{ text: `Получатель\n${result.orgName}`, colSpan: 2 }, {}, {}, {}],
                                    [{ text: `Банк получателя\n${result.orgBankName}`, colSpan: 2, rowSpan: 2 }, {}, 'БИК', result.orgBIK],
                                    [{}, {}, 'Сч. №', result.orgCorrAccount]
                                ]
                            },
                            style: 'mediumText',
                            margin: [0, 20, 0, 0]
                        },
                        {
                            text: `Счет № ${result.receiptNumber} от ${result.issuedDateTime}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `Исполнитель: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Заказчик: ${result.payerName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `ИНН: ${result.payerITN}, КПП: ${result.payerKPP}, Адрес: ${result.payerAddress}`,
                            style: 'mediumText'
                        },
                        {
                            table: {
                                headerRows: 1,
                                widths: [20, 220, 45, 'auto', 65, '*'],
                                body: bodyData
                            },
                            style: 'mediumText',
                            margin: [0, 15, 0, 40]
                        },
                        {
                            stack: [
                                {
                                    text: result.paymentTemplateLabelStr,
                                    style: 'mediumText',    
                                },
                                {
                                    text: result.paymentTemplateStr,
                                    style: 'mediumText',                                    
                                    italics: true,   
                                    bold: true,                                 
                                    margin: [0, 0, 0, 15],
                                },
                                {
                                    text: `Всего наименований: 2, на сумму ${result.totalAmountStr} руб.`,
                                    style: 'mediumText'
                                },
                                {
                                    text: `Сумма прописью: ${rubles(result.totalAmount)}. Без НДС`,
                                    style: 'mediumText'
                                },
                                {
                                    columns: [
                                        { text: 'Руководитель', margin: [0, 40, 0, 0]},
                                        { image: `${result.signatureImage}`, width: 150, alignment: 'center', margin: [25, 0, 25, 0] },
                                        { text: `${result.orgHeadName}`, margin: [0, 40, 0, 0] },
                                        { image: `${result.stampImage}`, width: 125, alignment: 'center', margin: [25, 0, 45, 0] }
                                    ],
                                    style: 'mediumText',
                                    margin: [0, 20, 0, 0]
                                }
                            ],
                            id: 'NoBreak'
                        }
                    ],

                    pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                        if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                          return true;
                        }
                        return false;
                    }, 

                    styles: {
                        headerWarning: {
                            fontSize: 7,
                            alignment: 'center'
                        },
                        mediumText: {
                            fontSize: 10,
                        },
                        smallText: {
                            fontSize: 8
                        },
                        tableHeader: {
                            bold: true,
                            fontSize: 10,
                            alignment: 'center'
                        }
                    }
                };

                pdfMake.createPdf(docDefinition).download(`${result.receiptNumber} ${result.payerName}`);
            },
            error: function (error) {
                console.log(error.message);
            }
        });
    });

    $(document).on('click',
        ".downloadReportPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [
                            {
                                text: result.orgName,
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                text: `Адрес: ${result.orgAddress}`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `АКТ № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `передачи документов`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [220, 65, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],
            
                                            body: [
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [`Адрес: ${result.orgAddress}`,`Адрес: ${result.payerAddress}`],
                                                [`Расчетный счет: ${result.orgFinancialAccount}`,`Расчетный счет: ${result.payerFinancialAccount}`],
                                                [`Кор. счет: ${result.orgCorrAccount}`,`Кор. счет: ${result.payerCorrAccount}`],
                                                [`Банк: ${result.orgBankName}`,`Банк: ${result.payerBankName}`],
                                                [`ИНН: ${result.orgITN}`,`ИНН: ${result.payerITN}`],
                                                [`КПП: ${result.orgKPP}`,`КПП: ${result.payerKPP}`],
                                                [`БИК: ${result.orgBIK}`,`БИК: ${result.payerBIK}`],
                                                [{},{}],
                                                [`Сдал             ${result.orgHeadName}`,`Принял ______________________________________`],
                                                [
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    },
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    }
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',
                                pageBreak: 'before',
                                bold: true
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                            },
                            mediumText: {
                                fontSize: 10,
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).download(`${result.receiptNumber} ${result.payerName}`);
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

    $(document).on('click',
        ".createReportPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [
                            {
                                text: result.orgName,
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                text: `Адрес: ${result.orgAddress}`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `АКТ № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `передачи документов`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [220, 65, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],
            
                                            body: [
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [`Адрес: ${result.orgAddress}`,`Адрес: ${result.payerAddress}`],
                                                [`Расчетный счет: ${result.orgFinancialAccount}`,`Расчетный счет: ${result.payerFinancialAccount}`],
                                                [`Кор. счет: ${result.orgCorrAccount}`,`Кор. счет: ${result.payerCorrAccount}`],
                                                [`Банк: ${result.orgBankName}`,`Банк: ${result.payerBankName}`],
                                                [`ИНН: ${result.orgITN}`,`ИНН: ${result.payerITN}`],
                                                [`КПП: ${result.orgKPP}`,`КПП: ${result.payerKPP}`],
                                                [`БИК: ${result.orgBIK}`,`БИК: ${result.payerBIK}`],
                                                [{},{}],
                                                [`Сдал             ${result.orgHeadName}`,`Принял ______________________________________`],
                                                [
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    },
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    }
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',
                                pageBreak: 'before',
                                bold: true
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                            },
                            mediumText: {
                                fontSize: 10,
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

        $(document).on('click',
        ".createIndustriaReportPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [
                            {
                                text: result.orgName,
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                text: `Адрес: ${result.orgAddress}`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `АКТ № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `передачи документов`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `На основании договора №${result.counterpartyDocument} от ${result.counterpartyDocumentDate}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [220, 65, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],
            
                                            body: [
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [`Адрес: ${result.orgAddress}`,`Адрес: ${result.payerAddress}`],
                                                [`Расчетный счет: ${result.orgFinancialAccount}`,`Расчетный счет: ${result.payerFinancialAccount}`],
                                                [`Кор. счет: ${result.orgCorrAccount}`,`Кор. счет: ${result.payerCorrAccount}`],
                                                [`Банк: ${result.orgBankName}`,`Банк: ${result.payerBankName}`],
                                                [`ИНН: ${result.orgITN}`,`ИНН: ${result.payerITN}`],
                                                [`КПП: ${result.orgKPP}`,`КПП: ${result.payerKPP}`],
                                                [`БИК: ${result.orgBIK}`,`БИК: ${result.payerBIK}`],
                                                [{},{}],
                                                [`Сдал             ${result.orgHeadName}`,`Принял ______________________________________`],
                                                [
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    },
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    }
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',
                                pageBreak: 'before',
                                bold: true
                            },
                            {
                                text: `На основании договора №${result.counterpartyDocument} от ${result.counterpartyDocumentDate}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 24, 8, 8],
            
                                            body: [
                                                [
                                                    {text: `ИСПОЛНИТЕЛЬ`, bold: true},
                                                    {text: `ЗАКАЗЧИК`, bold: true}
                                                ],
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [`Адрес: ${result.orgAddress}`,`Адрес: ${result.payerAddress}`],
                                                [`Расчетный счет: ${result.orgFinancialAccount}`,`Расчетный счет: ${result.payerFinancialAccount}`],
                                                [`Кор. счет: ${result.orgCorrAccount}`,`Кор. счет: ${result.payerCorrAccount}`],
                                                [`Банк: ${result.orgBankName}`,`Банк: ${result.payerBankName}`],
                                                [`ИНН: ${result.orgITN}`,`ИНН: ${result.payerITN}`],
                                                [`КПП: ${result.orgKPP}`,`КПП: ${result.payerKPP}`],
                                                [`БИК: ${result.orgBIK}`,`БИК: ${result.payerBIK}`],
                                                [{},{}],
                                                [
                                                    {text: `${result.orgHeadTitle} ${result.orgName}`},
                                                    {text: `${result.payerName}`}
                                                ],
                                                [
                                                    {text: `${result.orgHeadName}`},
                                                    {text: ``}
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                            },
                            mediumText: {
                                fontSize: 10,
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

        $(document).on('click',
        ".createReport2PDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptAvibaPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;

                    dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                    dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                    dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });                        
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 3, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.totalAmountStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [
                            {
                                text: result.orgName,
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                text: `Адрес: ${result.orgAddress}`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `АКТ № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `передачи документов`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [280, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],
            
                                            body: [
                                                ['',''],
                                                [`Сдал             ${result.orgHeadName}`,`Принял ______________________________________`],
                                                [
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    },
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    }
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',
                                pageBreak: 'before',
                                bold: true
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                            },
                            mediumText: {
                                fontSize: 10,
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

        $(document).on('click',
        ".createAvibaReportPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptAvibaPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [
                            { 
                                image: `${result.headerImage}`,
                                margin: [-40, -40, 0, 0],
                                width: 595.28
                            },
                            {
                                text: `АКТ передачи документов № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'bigText',
                                bold: true,
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [220, 65, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8],
            
                                            body: [
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [{},{}],
                                                [`Сдал             ${result.orgHeadName}`,`Принял ______________________________________`],
                                                [
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    },
                                                    {
                                                        text: `М.П.`, alignment: 'center'
                                                    }
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            { 
                                image: `${result.headerImage}`,
                                margin: [-40, -40, 0, 0],
                                width: 595.28,
                                pageBreak: 'before',
                            },                            
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',                                
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 13,
                                color: '#012659'
                            },
                            mediumText: {
                                fontSize: 10,
                                color: '#012659'
                            },
                            smallText: {
                                fontSize: 8,
                                color: '#012659'
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center',
                                color: '#012659'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

        $(document).on('click',
        ".createAvibaReportPDF2",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptAvibaPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    dataRow = [];
    
                    itemCount++;
                    dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                    dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                    dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                    dataRow.push({ text: 'полетный сегмент', alignment: 'center' });
                    dataRow.push({ text: '', alignment: 'right' });
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    feeData.push(dataRow);

                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [                            
                            { 
                                image: `${result.headerImage}`,
                                margin: [-40, -40, 0, 0],
                                width: 595.28,
                            },                            
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',                                
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 180, 45, 'auto', 55, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.totalAmountStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг на сумму ${result.totalAmountStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.totalAmountStr)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 13,
                                color: '#012659'
                            },
                            mediumText: {
                                fontSize: 10,
                                color: '#012659'
                            },
                            smallText: {
                                fontSize: 8,
                                color: '#012659'
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center',
                                color: '#012659'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

        $(document).on('click',
        ".createUstekReportPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptAvibaPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    dataRow = [];
    
                    itemCount++;
                    dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                    dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                    dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                    dataRow.push({ text: 'полетный сегмент', alignment: 'center' });
                    dataRow.push({ text: '', alignment: 'right' });
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    feeData.push(dataRow);

                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }

                    const docDefinition = {
                        info: {
                            title: 'test',
                        },
                        content: [                         
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',                                
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 180, 45, 'auto', 55, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.totalAmountStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг на сумму ${result.totalAmountStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.totalAmountStr)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        columns: [
                                            [
                                                {
                                                    text: 'ИСПОЛНИТЕЛЬ',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.orgHeadTitle} ${result.orgName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: `${result.orgHeadName}`,
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ],
                                            [
                                                {
                                                    text: 'ЗАКАЗЧИК',
                                                    bold: true,
                                                },
                                                {
                                                    text: `${result.payerName}`,
                                                    margin: [0, 10, 0, 0]
                                                },
                                                {
                                                    text: '________________________________________',
                                                    margin: [0, 15, 0, 0]
                                                }
                                            ]
                                        ],
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 13
                            },
                            mediumText: {
                                fontSize: 10
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

    $(document).on('click',
        ".createReportPDF2",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    console.log(result);
                    const itemData = [];

                    var headerRow = [];

                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    itemData.push(headerRow);

                    var dataRow = [];
                    var itemCount = 0;
                    result.items.forEach(function (item) {
                        dataRow = [];

                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
                        dataRow.push({ text: item.amountStr, alignment: 'right' });

                        itemData.push(dataRow);
                    });

                    result.luggageItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    });

                    if(result.items.length == 0 && result.luggageItems.length == 0) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: 'Авиабилеты', style: 'smallText' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'center' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                        dataRow.push({ text: result.itemTotalStr, alignment: 'right' });
    
                        itemData.push(dataRow);
                    }

                    dataRow = [];

                    dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({});
                    dataRow.push({ text: result.itemTotalStr, alignment: 'right' });

                    itemData.push(dataRow);

                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№', style: 'tableHeader' });
                    headerRow.push({ text: 'Наименование', style: 'tableHeader' });
                    headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
                    headerRow.push({ text: 'Ед.', style: 'tableHeader' });
                    headerRow.push({ text: 'Цена', style: 'tableHeader' });
                    headerRow.push({ text: 'Сумма', style: 'tableHeader' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.taxes.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
                        dataRow.push({ text: item.ticketLabel, style: 'smallText' });
                        dataRow.push({ text: item.segCount, alignment: 'center' });
                        dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
                        dataRow.push({ text: item.feeStr, alignment: 'right' });
                        dataRow.push({ text: item.amountStr, alignment: 'right' });
    
                        feeData.push(dataRow);
                    });

                    if(result.taxes.length == 0) {
                        dataRow = [];

                        dataRow.push({ text: '1', alignment: 'center' });
                        dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                        dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                        dataRow.push({ text: 'шт.', alignment: 'center' });
                        dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                        dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                        feeData.push(dataRow);
                    }
                    
                    // dataRow = [];

                    // dataRow.push({ text: '1', alignment: 'center' });
                    // dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                    // dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                    // dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                    // dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                    // dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                    // feeData.push(dataRow);

                    const docDefinition = {
                        content: [
                            {
                                text: result.orgName,
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                text: `Адрес: ${result.orgAddress}`,
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `АКТ № ${result.receiptNumber}-П от ${result.issuedDateTime}`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `передачи документов`,
                                style: 'mediumText',
                                bold: true,
                                alignment: 'center',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: `${result.orgName} передал, а ${result.payerName} принял следующие документы:`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [220, 65, 45, 'auto', '*'],
                                    body: itemData
                                },
                                style: 'mediumText',
                                margin: [0, 15, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Итого передано документов на сумму: ${rubles(result.itemTotal)}. Без НДС`,
                                        style: 'mediumText'
                                    },
                                    {                                        
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],
            
                                            body: [
                                                [`Передал: ${result.orgName}`,`Принял: ${result.payerName}`],
                                                [`Адрес: ${result.orgAddress}`,`Адрес: ${result.payerAddress}`],
                                                [`Расчетный счет: ${result.orgFinancialAccount}`,`Расчетный счет: ${result.payerFinancialAccount}`],
                                                [`Кор. счет: ${result.orgCorrAccount}`,`Кор. счет: ${result.payerCorrAccount}`],
                                                [`Банк: ${result.orgBankName}`,`Банк: ${result.payerBankName}`],
                                                [`ИНН: ${result.orgITN}`,`ИНН: ${result.payerITN}`],
                                                [`КПП: ${result.orgKPP}`,`КПП: ${result.payerKPP}`],
                                                [`БИК: ${result.orgBIK}`,`БИК: ${result.payerBIK}`],
                                                [{},{}],
                                                [
                                                    [
                                                        { 
                                                            image: `${result.signatureImage}`,
                                                            width: 150,
                                                            height: 60,
                                                            alignment: 'center',
                                                            margin: [35, 0, 0, 0]
                                                        },
                                                        {
                                                            text: `Сдал             ${result.orgHeadName}`,
                                                            margin: [0, -30, 0, 0]
                                                        }
                                                    ],
                                                    {
                                                        text: `Принял ______________________________________`,
                                                        margin: [0, 30, 0, 0]
                                                    }
                                                ],
                                                [                                                       
                                                    {
                                                        text: `М.П.`,
                                                        alignment: 'center'                                                        
                                                    },
                                                    {
                                                        text: `М.П.`,
                                                        alignment: 'center'
                                                    }
                                                ],
                                                [
                                                    {
                                                        image: `${result.stampImage}`,
                                                        width: 125,
                                                        alignment: 'center', margin: [25, 0, 45, 0]
                                                    },
                                                    {}
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',
                                        style: 'mediumText',
                                        margin: [0, 10, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: `АКТ № ${result.receiptNumber} от ${result.issuedDateTime}`,
                                style: 'bigText',
                                pageBreak: 'before',
                                bold: true
                            },
                            {
                                text: `Исполнитель: ${result.orgName}`,
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Заказчик: ${result.payerName}`,
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [20, 220, 45, 'auto', 65, '*'],
                                    body: feeData
                                },
                                style: 'mediumText',
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Итого: ${result.feeTotalStr}`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 35, 0, 0]
                            },
                            {
                                text: `Без налога (НДС): -`,
                                style: 'mediumText',
                                alignment: 'right',
                                bold: true,
                                margin: [0, 0, 0, 40]
                            },
                            {
                                stack: [
                                    {
                                        text: `Всего оказано услуг ${result.segCountTotal}, на сумму ${result.feeTotalStr} руб.`,
                                        style: 'mediumText'
                                    },
                                    {
                                        text: `${rubles(result.feeTotal)}`,
                                        style: 'mediumText',
                                        bold: true
                                    },
                                    {
                                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    },
                                    {
                                        table: {
                                            widths: ['*', '*'],
                                            heights: [8, 8, 8],
                                            body: [
                                                [
                                                    {
                                                        text: 'ИСПОЛНИТЕЛЬ',
                                                        bold: true,
                                                    },
                                                    {
                                                        text: 'ЗАКАЗЧИК',
                                                        bold: true,
                                                    }
                                                ],
                                                [
                                                    {
                                                        text: `${result.orgHeadTitle} ${result.orgName}`,
                                                        margin: [0, 10, 0, 0]
                                                    },
                                                    {
                                                        text: `${result.payerName}`,
                                                        margin: [0, 10, 0, 0]
                                                    }
                                                ],
                                                [   
                                                    [
                                                        { 
                                                            image: `${result.signatureImage}`,
                                                            width: 150,
                                                            height: 60,
                                                            margin: [15, 15, 0, 0]
                                                        },
                                                        {
                                                            text: `${result.orgHeadName}`,
                                                            margin: [0, -30, 0, 0]
                                                        },
                                                        {
                                                            image: `${result.stampImage}`,
                                                            width: 125,
                                                            margin: [25, 20, 0, 0]
                                                        }
                                                    ],
                                                    {
                                                        text: '________________________________________',
                                                        margin: [0, 45, 0, 0]
                                                    }                                                 
                                                ]
                                            ]
                                        },
                                        layout: 'noBorders',                                     
                                        style: 'mediumText',
                                        margin: [0, 25, 0, 0]
                                    }
                                ],
                                id: 'NoBreak'
                            }                       
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                            },
                            mediumText: {
                                fontSize: 10,
                            },
                            smallText: {
                                fontSize: 8
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

    $(document).on('click',
        ".showCorporatorDocument",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpClient/CorporatorDocumentPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).parent().parent().find(".docId").val() },
                success: function (result) {                    
                    const feeData = [];

                    headerRow = [];

                    headerRow.push({ text: '№ п/п', style: 'mediumText' });
                    headerRow.push({ text: 'Наименование', style: 'mediumText' });
                    headerRow.push({ text: 'Сумма', style: 'mediumText' });

                    feeData.push(headerRow);

                    itemCount = 0;
                    result.feeItems.forEach(function(item) {
                        dataRow = [];
    
                        itemCount++;
                        dataRow.push({ text: (itemCount).toString(), style: 'mediumText' });
                        dataRow.push({ text: item.name, style: 'mediumText' });
                        dataRow.push({ text: item.feeStr, style: 'mediumText' });
    
                        feeData.push(dataRow);
                    });

                    const docDefinition = {
                        content: [
                            {
                                text: `Договор № ${result.docNum} на оформление авиа и ж/д перевозок и оказание услуг`,
                                style: 'mediumText',
                                alignment: 'center',
                                bold: true
                            },
                            {
                                table: {
                                    widths: ['*', '*'],
                                    heights: [10],
                                    body: [
                                        [
                                            {
                                                text: 'г. Белгород',
                                                bold: true,
                                                alignment: 'left'
                                            },
                                            {
                                                text: result.docDate,
                                                bold: true,
                                                alignment: 'right'
                                            }
                                        ]
                                    ]
                                },
                                layout: 'noBorders',                                     
                                style: 'mediumText',
                                margin: [0, 25, 0, 0]
                            },
                            {
                                text: `${result.organizationName} - аккредитованное в Транспортной Клиринговой Палате РФ и в Международной Ассоциации Воздушного Транспорта агентство, в лице ${result.orgManagementPositionGenitive} ${result.orgManagementNameGenitive}, действующего на основании Устава, именуемое в дальнейшем «Исполнитель», с одной стороны и ${result.corporatorName}, в лице ${result.managementPositionGenitive} ${result.managementNameGenitive}, действующего на основании Устава, именуемое в дальнейшем «Заказчик», с другой стороны, вместе именуемые «Стороны», заключили настоящий Договор о нижеследующем:`,
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: '1. ОПРЕДЕЛЕНИЯ И ТОЛКОВАНИЕ',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0]
                            },
                            {
                                stack:[
                                    {
                                        text: `Для целей настоящего Договора (включая все приложения, изменения и дополнения):`                                 
                                    },
                                    {
                                        ul: [
                                            'Авиабилет – договор перевозки воздушным транспортом между физическим лицом, указанным в заявке Заказчика, и перевозчиком.',
                                            'Сегмент - часть маршрута, которая включает один перелет между двумя пунктами.',
                                            'Железнодорожный билет – договор перевозки железнодорожным транспортом между физическим лицом, указанным в заявке Заказчика, и перевозчиком.',
                                            'Услуги – действия Исполнителя по оформлению, бронированию, возврату железнодорожных билетов или авиабилетов на рейсы (отправления) по России, СНГ и дальнего зарубежья, а также иные действия, указанные в Прейскуранте Исполнителя, на выполнение которых Заказчиком была подана Заявка;',
                                            'Заявка – запрос Заказчика на оформление перевозок или отказ от оформленных перевозок, составленный по форме Приложения №1 к настоящему Договору, направленный Исполнителю посредством электронной почты.',
                                            'Заявление на возврат денежных средств в связи с отказом от перевозки – документ, составленный Заказчиком в произвольной форме.',
                                            'Перевозчик – организация, осуществляющая перевозки пассажиров авиационным и (или) железнодорожным транспортом в порядке и на основаниях, предусмотренных действующим законодательством.',
                                            'Тарифы и сборы Перевозчика – установленные расценки Перевозчика на авиа и железнодорожные перевозки, отказ от заключенных договоров перевозки, действующие на момент предоставления Заявки Заказчиком Исполнителю в письменной форме.',
                                            `Прейскурант Исполнителя (Приложение №2) – документ, утверждаемый Исполнителем, в котором указаны услуги Исполнителя обязательные для выполнения в рамках настоящего договора и размер оплаты, которую обязан произвести Заказчик за выполнение этих услуг.
                                            Заголовки в настоящем Договоре приведены исключительно для удобства и не влияют на значение и толкование настоящего Договора. Термины, использованные во множественном числе, включают свое значение в единственном числе и наоборот, если по контексту не требуется иное.`
                                        ]
                                    }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: '2. ПРЕДМЕТ ДОГОВОРА',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0]
                            },
                            {
                                stack: [
                                    { text: '2.1. По настоящему договору Исполнитель обязуется от имени и за счет Перевозчика заключить договор воздушной перевозки и/или железнодорожной перевозки с физическим(и) лицом(ами), согласно предоставленной Заказчиком заявки. Заказчик обязуется оплатить заключенные договоры перевозки и услуги Исполнителя.' },
                                    { text: '2.2. Перечень и стоимость Услуг определяется Прейскурантом Исполнителя.' }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: '3. ПОРЯДОК ОКАЗАНИЯ УСЛУГ',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0]
                            },
                            {
                                stack: [
                                    { text: `3.1. Заявка, составленная Заказчиком по форме Приложения № 1, которое является неотъемлемой частью Договора, направляется Исполнителю посредством электронной почты с адреса ${result.email}, принадлежащего ${result.corporatorName} на адрес aero3@aviba.ru с 09 до 18 часов местного времени ежедневно. Документы, отправленные в указанном выше порядке, признаются Сторонами и имеют статус официальных документов.` },
                                    { text: '3.2. Исполнитель в разумный срок после получения Заявки от Заказчика, но не позднее 20 часов текущего рабочего дня:' },
                                    { text: '3.2.1. Производит бронирование мест, оформляет авиабилеты или железнодорожные билеты, при наличии свободных мест в автоматизированных системах бронирования перевозчиков на указанные даты и по маршрутам, указанным в заявке Заказчика.' },
                                    { text: '3.2.2. Выполняет также иные действия, которые необходимо выполнить в соответствии с Заявкой для надлежащего оказания Услуг.' }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],
                                id: 'NoBreak'
                            },
                            {
                                pageBreak: 'before',
                                text: '4. ПОРЯДОК ОПЛАТЫ',
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                stack: [
                                    { text: `4.1. Денежные средства за проданные перевозки, в соответствии с договорами Исполнителя, являются собственностью Перевозчиков. Исполнитель обязан ежедневно перечислять выручку за проданные перевозки на счета Перевозчиков.` },
                                    { text: '4.2. Учитывая п.4.1, оплата Услуг, а также стоимость железнодорожных билетов и авиабилетов осуществляется Заказчиком в течение 1 (одного) банковского дня с момента получения Заказчиком счета Исполнителя путем перечисления денежных средств на расчетный счет Исполнителя, указанный в счете на оплату, либо за наличный расчет при получении билетов.' },
                                    { text: '4.3. Оплата в любом случае должна быть произведена в рабочий день, предшествующий дню начала выполнения перевозки.' },
                                    { text: '4.4. Обязательства по перечислению денежных средств считаются выполненными в момент зачисления денежных средств на расчетный счет Исполнителя.' },
                                    { text: '4.5. Оплата услуг, а также железнодорожных и авиабилетов может производиться посредством внесения аванса Заказчиком на расчетный счет Исполнителя в согласованном Сторонами размере. Исполнитель обязуется использовать данный аванс в зачет оплаты заключенных договоров перевозки и оказанных услуг согласно Заявкам Заказчика.' }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],
                            },
                            {
                                text: '5. ПРАВА И ОБЯЗАННОСТИ СТОРОН',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0],
                            },
                            {
                                stack: [
                                    { text: `5.1. При наличии задолженности у Заказчика перед Исполнителем, Исполнитель имеет право отказать Заказчику в выполнении заявки.` },
                                    { text: '5.2. При нарушении сроков оплаты, согласно п.4.1. настоящего Договора, Исполнитель в праве аннулировать договоры воздушных перевозок и/или железнодорожных перевозок, с удержанием с Заказчика сумм тарифов и сборов Перевозчика, предусмотренных за добровольный отказ от перевози и удержанием оплаты за услуги Исполнителя согласно Прейскуранту Исполнителя.' },
                                    { text: '5.3. Исполнитель имеет право в одностороннем порядке изменять стоимость услуг посредством уведомления Заказчика по электронной почте, адрес которой указан в п.3.1 настоящего Договора.' },
                                    { text: '5.4. В случае несогласия Заказчика с изменениями цен на услуги, Заказчик имеет право расторгнуть настоящий Договор в одностороннем порядке, о чем обязан уведомить Исполнителя в течении одного рабочего дня с момента получения документа об изменении стоимости услуг, путем отправки уведомления по электронной почте, адрес которой указан в п.3.1 настоящего договора.' },
                                    { text: '5.5. Заказчик обязуется:', bold: true },
                                    { text: '5.5.1. Направлять Заявки Исполнителю по форме Приложения № 1 к настоящему Договору.' },
                                    { text: '5.5.2. Своевременно оплачивать Услуги и стоимость билетов согласно тарифам Перевозчика и Прейскуранту Исполнителя.' },
                                    { text: '5.5.3. После получения маршрут- квитанций по электронной почте, проверить данные в маршрут- квитанциях (маршрут, число, время, аэропорты вылета и прилета, паспортные данные пассажиров) и подтвердить их правильность в сообщении, отправленном на электронную почту aero3@aviba.ru. В случае отсутствия подтверждения в течение 30 минут с момента отправки маршрут-квитанций Заказчику, данные считаются проверенными.' },
                                    { text: '5.6. Исполнитель обязуется:', bold: true },
                                    { text: '5.6.1. Принимать Заявки, предоставленные Заказчиком.' },
                                    { text: '5.6.2. Своевременно исполнять Заявки Заказчика, либо немедленно уведомлять об отказе от исполнения с указанием уважительных причин невозможности исполнения Заявки.' },
                                    { text: '5.6.3. Осуществлять оформление билетов по тарифам Перевозчика, действующим на момент предоставления Заявки в письменном виде.' },
                                    { text: '5.6.4. Своевременно предоставлять Заказчику железнодорожные билеты и авиабилеты, иные отчетные документы, подтверждающие расходы, в том числе счета на оплату и акты выполненных работ (услуг) согласно Прейскуранту с указанием маршрута следования, ФИО пассажира и номера поезда/рейса на каждого работника, указанного в заявке Заказчика.' },
                                    { text: '5.6.5. В случае возврата билетов (отказа от перевозки), в срок не позднее 3-х банковских дней с момента получения от Заказчика Заявления на возврат денежных средств, возвратить ему стоимость билетов, удержав сборы Перевозчика за отказ от перевозки согласно условиям возврата Перевозчика, а также стоимость услуги за оформление возврата билетов на основании Прейскуранта. По желанию Заказчика денежные средства, предназначенные для возврата, остаются на счету исполнителя и будут использованы для зачета при оформлении последующих договоров перевозки.' },
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],                                
                                id: 'NoBreak'
                            },
                            {
                                pageBreak: 'before',
                                text: '6. СРОК ДЕЙСТВИЯ ДОГОВОРА',
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                stack: [
                                    { text: `6.1. Настоящий Договор вступает в силу с момента его подписания Сторонами и заключается до .` },
                                    { text: '6.2. В случае если за 1 месяц до истечения срока действия настоящего Договора ни одна из Сторон не направит другой Стороне письменное уведомление о расторжении Договора в связи с истечением его срока, Договор автоматически ежегодно пролонгируется на каждый следующий календарный год.' }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0]
                            },
                            {                                
                                text: '7. ОБСТОЯТЕЛЬСТВА НЕПРЕОДОЛИМОЙ СИЛЫ',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0],
                            },
                            {
                                stack: [
                                    { 
                                        stack: [
                                            { 
                                                text: '7.1. Ни одна из Сторон не несет ответственность за неисполнение или ненадлежащее исполнение своих обязанностей по настоящему Договору вследствие наступления после даты заключения настоящего Договора обстоятельств непреодолимой силы, к которым, наряду с прочими, относятся:' 
                                            },
                                            {
                                                ul: [ 
                                                    'наводнения, землетрясения и иные стихийные бедствия;',
                                                    'войны, военные действия, введение чрезвычайного положения;',
                                                    'иные аналогичные события, находящиеся вне разумного предвидения и контроля Сторон, которые в соответствии с действующим законодательством Российской Федерации относятся к обстоятельствам непреодолимой силы.',
                                                ],
                                            }
                                        ]
                                    },
                                    { text: '7.2. При наступлении обстоятельств непреодолимой силы Сторона, на которую распространяется действие таких обстоятельств, должна в течение 10 (десяти) дней представить другой Стороне соответствующее уведомление. В таком уведомлении должна содержаться информация о характере, дате наступления обстоятельств непреодолимой силы, а также о том, каким образом обстоятельства непреодолимой силы влияют на возможность соответствующей Стороны выполнить свои обязательства по настоящему Договору. Если Сторона, в отношении которой возникли обстоятельства непреодолимой силы, не представила уведомление в течение указанного срока, такая Сторона лишается права ссылаться на такие обстоятельства как на основание, исключающее ее ответственность за невыполнение или ненадлежащее выполнение своих обязательств по настоящему Договору.' },
                                    { text: '7.3. В случае наступления обстоятельств непреодолимой силы, при условии соблюдения соответствующей Стороной пункта 7.1. срок выполнения Стороной соответствующего обязательства по настоящему Договору отодвигается соразмерно времени, в течение которого действуют эти обстоятельства, если иное не вытекает из существа обязательства или не будет установлено Договором Сторон.' },
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],
                            },
                            {
                                text: '8. ОТВЕТСТВЕННОСТЬ СТОРОН',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0],
                            },
                            {
                                stack: [
                                    { text: `8.1. За неисполнение или ненадлежащее исполнение своих обязанностей по настоящему договору стороны несут ответственность в соответствии с действующим законодательством.` },
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                text: '9. ОСОБЫЕ УСЛОВИЯ',
                                style: 'mediumText',
                                bold: true,
                                margin: [0, 10, 0, 0],
                            },
                            {
                                stack: [
                                    { text: `9.1. Все дополнительные соглашения и приложения к настоящему Договору являются его неотъемлемой частью и оформляются в письменном виде и подписываются обеими сторонами.` },
                                    { text: `9.2. Каждая факсимильная или отправленная по электронной почте Заявка, уведомление и Прейскурант Исполнителя становятся неотъемлемой частью настоящего Договора.` },
                                    { text: `9.3. Стороны обязуются в срок не позднее 1(одного) рабочего дня извещать друг друга об изменении своих почтовых адресов, платёжных и иных реквизитов.` },
                                    { text: `9.4. В целях обмена почтовой корреспонденцией Стороны устанавливают следующие почтовые адреса и телефоны:` },
                                    { text: ' ' },
                                    { text: `9.4.1. Адрес Заказчика: ${result.corporatorName}, ${result.address}` },
                                    { text: `9.4.2. Адрес Исполнителя: ${result.organizationName}, ${result.orgAddress}` },
                                    { text: ' ' },
                                    { text: `9.5. Все споры, связанные с заключением, исполнением, расторжением настоящего Договора решаются путем проведения переговоров.` },
                                    { text: `9.6. Споры, не урегулированные в порядке переговоров, передаются на рассмотрение в Арбитражный суд Белгородской области.` },
                                    { text: `9.7. Настоящий Договор составлен в 2 экземплярах по одному для каждой из Сторон.` },
                                    { text: ' ' },
                                    { text: `Приложения:` },
                                    { text: `Форма Заявки (Приложение № 1).` },
                                    { text: `Прейскурант Исполнителя (Приложение №2).` }
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],
                                id: 'NoBreak'
                            },
                            {
                                pageBreak: 'before',
                                text: '10. АДРЕСА И РЕКВИЗИТЫ СТОРОН',
                                style: 'mediumText',
                                bold: true
                            },
                            {
                                table: {
                                    widths: ['45%', '5%', '45%'],
                                    body: [
                                        [
                                            {
                                                stack: [
                                                    { text: 'ИСПОЛНИТЕЛЬ:', },
                                                    { text: result.organizationName, bold: true },
                                                    { text: [ { text: 'Адрес местонахождения: ', bold: true }, result.orgAddress] },
                                                    { text: [ { text: 'ИНН: ', bold: true }, result.orgITN] },
                                                    { text: [ { text: 'КПП: ', bold: true }, result.orgKPP] },
                                                    { text: [ { text: 'ОГРН: ', bold: true }, result.orgOGRN] },
                                                    { text: ' ' },
                                                    { text: [ { text: 'Р/c: ', bold: true }, `${result.orgAccountDescription} в ${result.orgAccountAddress}`] },
                                                    { text: [ { text: 'К/с: ', bold: true }, result.orgCorrespondentAccount] },
                                                    { text: [ { text: 'БИК: ', bold: true }, result.orgBIK] },
                                                    { text: [ { text: 'Телефон: ', bold: true }, result.orgPhone] },
                                                ]
                                            },
                                            {
                                                text: ' '
                                            },
                                            {
                                                stack: [
                                                    { text: 'ЗАКАЗЧИК:', },
                                                    { text: result.corporatorName, bold: true },
                                                    { text: [ { text: 'Адрес местонахождения: ', bold: true }, result.address] },
                                                    { text: [ { text: 'ИНН: ', bold: true }, result.itn] },
                                                    { text: [ { text: 'КПП: ', bold: true }, result.kpp] },
                                                    { text: [ { text: 'ОГРН: ', bold: true }, result.ogrn] },
                                                    { text: ' ' },
                                                    { text: [ { text: 'Р/c: ', bold: true }, `${result.corporatorAccountDescription} в ${result.corporatorAccountAddress}`] },
                                                    { text: [ { text: 'К/с: ', bold: true }, result.correspondentAccount] },
                                                    { text: [ { text: 'БИК: ', bold: true }, result.bik] },
                                                    { text: [ { text: 'Телефон: ', bold: true }, result.phone] },
                                                ]
                                            }
                                        ]
                                    ]
                                },
                                layout: 'noBorders',                                     
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                table: {
                                    widths: ['45%', '5%', '45%'],
                                    body: [
                                        [
                                            {
                                                stack: [
                                                    { text: result.orgManagementPosition, },
                                                    { text: result.organizationName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.orgManagementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            },
                                            {
                                                text: ' '
                                            },
                                            {
                                                stack: [
                                                    { text: result.managementPosition, },
                                                    { text: result.corporatorName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.managementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            }
                                        ]
                                    ]
                                },
                                layout: 'noBorders',                                     
                                style: 'mediumText',
                                margin: [0, 10, 0, 0]
                            },
                            {
                                table: {
                                    widths: ['45%', '5%', '45%'],
                                    body: [
                                        [
                                            {
                                                stack: [
                                                    { text: 'Утверждаю:', },
                                                    { text: result.orgManagementPosition, },
                                                    { text: result.organizationName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.orgManagementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            },
                                            {
                                                text: ' '
                                            },
                                            {
                                                stack: [
                                                    { text: 'Утверждаю:', },
                                                    { text: result.managementPosition, },
                                                    { text: result.corporatorName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.managementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            }
                                        ]
                                    ]
                                },
                                layout: 'noBorders',                                     
                                style: 'mediumText',
                                margin: [0, 120, 0, 0],
                                id: 'NoBreak'
                            },
                            {
                                stack: [
                                    { text: `Приложение № 1 (Заявка) к Договору на оформление` },
                                    { text: `авиа и ж/д перевозок и оказание услуг` },
                                    { text: `№ ${result.docNum} от ${result.docDate}` }
                                ],
                                style: 'mediumText',
                                alignment: 'right',
                                pageBreak: 'before',
                            },
                            {
                                stack: [
                                    { text: `(Форма Заявки на бланке заказчика)` },
                                    { text: `Заявка на предоставление услуг (оформление/отказ от перевозки)` },
                                    { text: ' ' },
                                    { text: ' ' },
                                    { text: `Маршруты следования:` }
                                ],
                                style: 'mediumText',
                                alignment: 'left',
                                margin: [0, 10, 0, 0],
                            },
                            {
                                canvas: [ 
                                    { type: 'line', x1: 0, y1: 0, x2: 515, y2: 0, lineWidth: 1, color: '#001f66' } 
                                ],
                                margin: [0, 20, 0, 0],
                            },
                            {
                                stack: [
                                    { text: `Дополнительные сведения (время вылета/отправления, время прилета/прибытия, рейс/№поезда, желаемое место и др.) заполняется при необходимости.` },
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 10, 0, 0],
                            },
                            {
                                canvas: [ 
                                    { type: 'line', x1: 0, y1: 0, x2: 515, y2: 0, lineWidth: 1, color: '#001f66' } 
                                ],
                                margin: [0, 20, 0, 0],
                            },
                            {
                                canvas: [ 
                                    { type: 'line', x1: 0, y1: 0, x2: 515, y2: 0, lineWidth: 1, color: '#001f66' } 
                                ],
                                margin: [0, 20, 0, 0],
                            },
                            {
                                table: {
                                    widths: ['20%', '30%', '15%', '15%', '20%'],
                                    heights: [30, 20, 20, 20],
                                    body: [
                                        [
                                            {
                                                stack: [
                                                    { text: 'Ф.И.О.', },
                                                    { text: 'Пассажира', },
                                                    { text: '(как в документе)' }
                                                ],
                                                alignment: 'center'
                                            },
                                            {
                                                stack: [
                                                    { text: 'Дата рождения', },
                                                    { text: '(для ж/д билетов', },
                                                    { text: 'указать место рождеия)' }
                                                ],
                                                alignment: 'center'
                                            },
                                            {
                                                stack: [
                                                    { text: 'Тип документа', },
                                                    { text: 'Серия и номер', }
                                                ],
                                                alignment: 'center'
                                            },
                                            {
                                                stack: [
                                                    { text: 'Срок действия', },
                                                    { text: 'документа', },
                                                    { text: '(если есть)' }
                                                ],
                                                alignment: 'center'
                                            },
                                            {
                                                stack: [
                                                    { text: 'Дата', },
                                                    { text: 'вылета/отправления', },
                                                    { text: 'класс обслуживания' }
                                                ],
                                                alignment: 'center'
                                            },
                                        ],
                                        [{},{},{},{},{}],
                                        [{},{},{},{},{}],
                                        [{},{},{},{},{}],
                                    ]
                                },                                  
                                style: 'pt8',                                
                                margin: [0, 20, 0, 0],
                            },
                            {
                                text: 'Перелеты за пределы РФ, для граждан России, осуществляются только по загран. паспорту РФ',
                                style: 'mediumText'
                            },
                            {
                                stack: [
                                    { text: `Подпись ответственного лица Заказчика, печать Заказчика` },
                                    { text: ' ' },
                                    { text: ' ' },
                                    { text: '___________________________/___________________________/' },
                                    { text: 'расшифровка подписи', margin: [130, 0, 0, 0] },
                                    { text: ' ' },
                                    { text: ' ' },
                                    { text: 'Телефон для связи ___________________________________' },
                                ],
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 30, 0, 0],
                                id: 'NoBreak'
                            },
                            {
                                stack: [
                                    { text: `Приложение №2 (Прейскурант Исполнителя) к Договору` },
                                    { text: `на оформление авиа и ж/д перевозок и оказание услуг` },
                                    { text: `№ ${result.docNum} от ${result.docDate}` }
                                ],
                                style: 'mediumText',
                                alignment: 'right',
                                pageBreak: 'before',
                            },
                            {
                                text: `${result.organizationName}, являющееся юридическим лицом по законодательству Российской Федерации, в лице ${result.orgManagementPositionGenitive} ${result.orgManagementNameGenitive}, действующего на основании Устава, именуемое в дальнейшем «Исполнитель», с одной стороны и ${result.corporatorName}, в лице ${result.managementPositionGenitive} ${result.managementNameGenitive}, действующего на основании Устава, именуемое в дальнейшем «Заказчик», с другой стороны, принимая во внимание, что в стоимость выставляемых Исполнителем счетов входит стоимость Услуг и стоимость билета, которая соответствует тарифам Перевозчика на день предоставления Заявки Заказчиком, утвердили следующие тарифы на Услуги:`,
                                style: 'mediumText',
                                alignment: 'justify',
                                margin: [0, 40, 0, 0]
                            },
                            {
                                table: {
                                    headerRows: 1,
                                    widths: [30, '*', 150],
                                    body: feeData
                                },
                                layout: {
                                    paddingTop: function(i, node) { return 5; },
                                    paddingBottom: function(i, node) { return 5; }
                                },
                                style: 'mediumText',
                                margin: [0, 20, 0, 0]
                            },
                            {
                                table: {
                                    widths: ['45%', '5%', '45%'],
                                    body: [
                                        [
                                            {
                                                stack: [
                                                    { text: 'ИСПОЛНИТЕЛЬ:', },
                                                    { text: ' ' },
                                                    { text: result.orgManagementPosition, },
                                                    { text: result.organizationName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.orgManagementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            },
                                            {
                                                text: ' '
                                            },
                                            {
                                                stack: [
                                                    { text: 'ЗАКАЗЧИК:', },
                                                    { text: ' ' },
                                                    { text: result.managementPosition, },
                                                    { text: result.corporatorName },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: ' ' },
                                                    { text: `__________________________/ ${result.managementName} /`},
                                                    { text: ' ' },
                                                    { text: 'М.П.' },
                                                ]
                                            }
                                        ]
                                    ]
                                },
                                layout: 'noBorders',                                     
                                style: 'mediumText',
                                margin: [0, 50, 0, 0],
                            }
                        ],

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        },                        

                        styles: {
                            bigText: {
                                fontSize: 12,
                                color: '#001f66'
                            },
                            pt11: {
                                fontSize: 11,
                                color: '#001f66'
                            },
                            mediumText: {
                                fontSize: 10,
                                color: '#001f66'
                            },
                            pt8: {
                                fontSize: 8,
                                color: '#001f66'
                            },
                            tableHeader: {
                                bold: true,
                                fontSize: 10,
                                alignment: 'center'
                            }
                        }
                    };

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
            });
        });

    $(document).on('click',
        ".createReceiptText",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpReceipt/ReceiptPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    console.log(result);

                    var receiptText = `Оплата по сч. N ${result.receiptNumber} от ${result.issuedDateTime}<br/>
                    Сумма: ${result.totalAmountStr}р<br/>
                    Без НДС <br/>
                    ${result.orgName}<br/>
                    ИНН: ${result.orgITN}<br/>
                    КПП: ${result.orgKPP}<br/>
                    ${result.orgAddress}<br/>
                    ${result.orgBankName}<br/>
                    БИК: ${result.orgBIK}<br/>
                    Кор/сч.: ${result.orgCorrAccount}<br/>
                    Р/сч.: ${result.orgFinancialAccount}`;

                    var win = window.open('', '_blank');
                    win.document.write(receiptText);
                },
                error: function (error) {
                    console.log(error.message);
                }
        });
    });

    $(document).on('click',
        ".createTicketsPDF",
        function (e) {
            e.preventDefault();
            $.ajax({
                url: "/CorpClient/TicketPDFData",
                type: "POST",
                cache: false,
                data: { id: $(this).closest('tr').find('.receiptId').val() },
                success: function (result) {
                    if(result.tickets.length == 0)
                        return;

                    const docDefinition = {
                        pageMargins: [ 0, 0, 0, 0 ],

                        background: {
                            image: '',
                            width: 600,
                        },

                        content: [],

                        defaultStyle: {
                            font: 'Helios',
                            color: '#002147'
                        },

                        styles: {
                            Calibri10: {
                                fontSize: 10
                            },
                            CalibriBd10: {
                                fontSize: 10,
                                bold: true
                            },
                            Calibri12: {
                                fontSize: 12
                            },
                            CalibriBd12: {
                                fontSize: 12,
                                bold: true
                            },
                            Calibri11: {
                                fontSize: 11
                            },
                            CalibriBd11: {
                                fontSize: 11,
                                bold: true
                            }
                        },

                        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
                            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length != 1) {
                              return true;
                            }
                            return false;
                        }, 
                    };

                    console.log(docDefinition);
                        
                    var index = 0;

                    result.tickets.forEach(function(item) {
                        docDefinition.background = {
                            image: `${item.blankImage}`,
                            width: 600,
                        },

                        docDefinition.content = docDefinition.content.concat([
                            {
                                stack: [
                                    {
                                        text: ' ',
                                        margin: [ 0, 0, 0, 0]
                                    },
                                ],
                                id: 'NoBreak'
                            },
                            {
                                text: 'МАРШРУТ / КВИТАНЦИЯ ЭЛЕКТРОННОГО БИЛЕТА',
                                fontSize: 13,
                                italics: true,
                                margin: [ 252, 16, 0, 0 ],
                                color: '#192646'
                            },
                            {
                                text: item.ticketNumber,
                                fontSize: 11,
                                bold: true,
                                margin: [ 420, 8, 0, 0 ],
                                color: '#192646'
                            },
                            {
                                columns: [
                                    {                                    
                                        text: 'СВЕДЕНИЯ О ПАССАЖИРЕ',
                                        bold: true,
                                        width: 142                                        
                                    },
                                    {
                                        text: '/ PASSENGER INFORMATION'                                          
                                    }
                                ],
                                fontSize: 10,
                                color: '#002857',
                                margin: [ 18, 34, 0, 0 ]
                            },
                            {                                        
                                table: {
                                    widths: [338, 116, 100],
                                    heights: [15, 15],
    
                                    body: [
                                        [
                                            {
                                                text: 'Фамилия Имя Отчество',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Тип документа',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Номер документа',
                                                color: '#ffffff',
                                                bold: true
                                            }
                                        ],
                                        [item.passengerName,item.docType,item.doc]
                                    ]
                                },
                                fontSize: 9,     
                                color: '#002857',                               
                                margin: [ 18, 8, 0, 0 ],
                                layout: 'noBorders',
                            },
                            {
                                columns: [
                                    {                                    
                                        text: 'МАРШРУТ ПЕРЕЛЕТА',
                                        bold: true,
                                        width: 112                                        
                                    },
                                    {
                                        text: '/ THE ROUTE',
                                        width: 233                                       
                                    },
                                    {
                                        text: 'Указано местное время отправления/прибытия',
                                        color: '#009bd9'                                          
                                    }
                                ],
                                fontSize: 10,
                                color: '#002857',
                                margin: [ 18, 17, 0, 0 ]
                            },
                            {                                        
                                table: {
                                    widths: [155, 155, 48, 87, 87],
                                    heights: [13, 14, 14, 14, 14],
    
                                    body: [
                                        [
                                            {
                                                text: 'Аэропорт отправления',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Аэропорт прибытия',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Рейс',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Отправление',
                                                color: '#ffffff',
                                                bold: true
                                            },
                                            {
                                                text: 'Прибытие',
                                                color: '#ffffff',
                                                bold: true
                                            }
                                        ],
                                        [item.seg[0].origin,item.seg[0].destination,item.seg[0].flight,item.seg[0].departure,item.seg[0].arrival],
                                        [item.seg[1].origin,item.seg[1].destination,item.seg[1].flight,item.seg[1].departure,item.seg[1].arrival],
                                        [item.seg[2].origin,item.seg[2].destination,item.seg[2].flight,item.seg[2].departure,item.seg[2].arrival],
                                        [item.seg[3].origin,item.seg[3].destination,item.seg[3].flight,item.seg[3].departure,item.seg[3].arrival]
                                    ]
                                },
                                fontSize: 9,     
                                color: '#002857',                               
                                margin: [ 18, 5, 0, 0 ],
                                layout: 'noBorders',
                            },
                            {
                                columns: [
                                    {
                                        stack: [
                                            {                                    
                                                text: '03БЕД ТКП'                          
                                            },
                                            {                                    
                                                text: 'AVIBA.RU',
                                                margin: [0, 2, 0, 0]                         
                                            },
                                            {                                    
                                                text: 'БЕЛГОРОД РФ',
                                                margin: [0, 2, 0, 0]                         
                                            },
                                            {                                    
                                                text: item.stamp,
                                                margin: [0, 2, 0, 0]
                                            },
                                        ],
                                        width: 200,
                                        margin: [0, 5, 0, 0]
                                    },                                     
                                    {                                        
                                        table: {
                                            widths: [82, 80, 150],
                                            heights: [11, 11, 11, 11],
            
                                            body: [                                                    
                                                ['ВЫДАН ОТ','ISSUED BY',item.issuedBy],
                                                [
                                                    {
                                                        text: 'НОМЕР БИЛЕТА',
                                                        bold: true
                                                    },
                                                    'TICKET NUMBER',
                                                    {
                                                        text: item.ticketNumber,
                                                        bold: true
                                                    }],
                                                ['ДАННЫЕ БРОНИ','BOOKING REF',item.pnr],
                                                [
                                                    {
                                                        text: 'ДАТА ВЫДАЧИ',
                                                        bold: true
                                                    },
                                                    'DATE OF ISSUE',
                                                    {
                                                        text: item.dateOfIssue,
                                                        bold: true
                                                    }]
                                            ]
                                        },
                                        layout: 'noBorders',
                                    }
                                ],
                                fontSize: 9,
                                color: '#002857',
                                margin: [ 18, 42, 0, 0 ]
                            },
                            {
                                table: {
                                    widths: [550],
                                    heights: [120],

                                    body: [
                                        [
                                            {
                                                columns: [
                                                    {
                                                        stack: [
                                                            {                                    
                                                                columns: [
                                                                    {                                    
                                                                        text: 'РАСЧЕТ СТОИМОСТИ',
                                                                        bold: true,
                                                                        width: 113                                       
                                                                    },
                                                                    {
                                                                        text: '/ FARE CALCULATION'                                          
                                                                    }
                                                                ],
                                                                fontSize: 10   
                                                            },
                                                            {                                    
                                                                text: item.fareCalc,
                                                                margin: [0, 11, 0, 0],
                                                                lineHeight: 1.45,            
                                                            }
                                                        ],
                                                        width: 370
                                                    },                                     
                                                    {
                                                        stack: [
                                                            {                                    
                                                                columns: [
                                                                    {                                    
                                                                        text: 'БАГАЖ:',
                                                                        bold: true,
                                                                        width: 45                                       
                                                                    },
                                                                    {
                                                                        text: item.luggage                                        
                                                                    }
                                                                ],
                                                                fontSize: 10          
                                                            },
                                                            {                                    
                                                                columns: [
                                                                    {                                    
                                                                        text: 'СТ/ST:',
                                                                        bold: true,
                                                                        width: 32                                       
                                                                    },
                                                                    {
                                                                        text: item.status                                          
                                                                    }
                                                                ],
                                                                fontSize: 8,
                                                                margin: [0, 40, 0, 0]                         
                                                            },
                                                            {                                    
                                                                columns: [
                                                                    {                                    
                                                                        text: 'КЛ/CL:',
                                                                        bold: true,
                                                                        width: 32                                       
                                                                    },
                                                                    {
                                                                        text: item.class                                         
                                                                    }
                                                                ],
                                                                fontSize: 8,
                                                                margin: [0, 5, 0, 0]                        
                                                            }
                                                        ],
                                                        margin: [7, 0, 0, 0]
                                                    },
                                                ]
                                            }
                                        ]
                                    ]
                                },       
                                layout: 'noBorders',                                                            
                                fontSize: 9,
                                color: '#002857',
                                margin: [ 18, 16, 0, 0 ]
                            }, 
                            {
                                columns: [
                                    {
                                        text: 'Итого/Total',
                                        width: 65,
                                        color: '#ffffff',
                                        margin: [ 0, 6, 0, 0 ]
                                    },         
                                    {
                                        text: item.total,
                                        width: 140,
                                        margin: [ 0, 6, 0, 0 ]
                                    },                              
                                    {      
                                        stack: [
                                            {
                                                text: [
                                                    'ФОРМА ОПЛАТЫ',
                                                    {
                                                        text: ' / FORM OF PAYMENT',
                                                        bold: false
                                                    }
                                                ]                                                   
                                            },
                                            item.payment
                                        ]
                                    }
                                ],
                                fontSize: 9,
                                color: '#002857',
                                bold: true,
                                margin: [ 190, 8, 0, 0 ]
                            },
                            {
                                table: {
                                    heights: [30],

                                    body: [
                                        [   
                                            {
                                                text: [
                                                    `${item.isExchange?"ВЫДАН В ОБМЕН БИЛЕТА   № ":""}`,
                                                    {
                                                        text: `${item.isExchange?item.exTicketNumber:""}`,
                                                        bold:true
                                                    }                                                
                                                ]                                                  
                                            }
                                        ]
                                    ]                                    
                                },
                                fontSize: 9,
                                color: '#002857',
                                layout: 'noBorders',   
                                margin: [ 23, 20, 0, 0 ]
                            },
                            {
                                table: {
                                    widths: [70],
                                    heights: [70],

                                    body: [
                                        [
                                            {
                                                qr: item.qr,
                                                fit: 70                                                    
                                            }
                                        ]
                                    ]                                    
                                },
                                layout: 'noBorders',   
                                margin: [ 23, 100, 0, 0 ]
                            },
                            {
                                columns: [
                                    {
                                        text: 'Аккредитованное агентство ООО «АВИБА.РУ»',
                                        width: 305,
                                        fontSize: 13
                                    },         
                                    {
                                        stack: [
                                            'тел: 8-800-707-76-77, e-mail: office@aviba.ru',
                                            'WWW.AVIBA.RU'
                                        ],
                                        fontSize: 11,
                                        margin: [ 0, 1, 0, 0 ]
                                    }
                                ],                                    
                                color: '#002857',
                                margin: [ 24, 22, 0, 0 ]                                
                            }
                        ]);
                        
                        index++;
                    });

                    console.log(docDefinition);

                    pdfMake.createPdf(docDefinition).open();
                },
                error: function (error) {
                    console.log(error.message);
                }
        });
    });

        function numberWithSpaces(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        }

    

    $(document).on('input',
        '.ticketPayment',
        function(e) {
            var segTotal = 0;
            var ticketTotal = 0;
            var feeRate = Number($('#feeRate').val().replace(',', '.'));
            $("#receiptItemsTable tbody").children().each(function () {
                const firstDiv = $(this).find("input");
                if (firstDiv.length) {
                    segTotal += Number(firstDiv[1].value);
                    ticketTotal += Number(firstDiv[4].value.replace(/[^0-9.-]+/g, ""));
                }
            });

            $('#segTotal').html(numberWithSpaces(segTotal));
            $('#feeTotal').html(numberWithSpaces(segTotal * feeRate));
            $('#ticketTotal').html(numberWithSpaces(ticketTotal));
            $('#finalTotal').html(numberWithSpaces(segTotal * feeRate + ticketTotal));
        });

    $(document).on('input',
        '.ticketPaymentCorp',
        function(e) {
            var segTotal = 0;
            var ticketTotal = 0;
            var feeTotal = 0;
            $("#receiptItemsTable tbody").children().each(function () {
                const firstDiv = $(this).find("input");
                if (firstDiv.length) {
                    segTotal += Number(firstDiv[1].value);
                    ticketTotal += Number(firstDiv[6].value.replace(/[^0-9.-]+/g, ""));
                    feeTotal += Number(firstDiv[1].value) * Number(firstDiv[7].value.replace(/[^0-9.-]+/g, ""));
                }
            });

            $('#segTotal').html(numberWithSpaces(segTotal));
            $('#feeTotal').html(numberWithSpaces(feeTotal));
            $('#ticketTotal').html(numberWithSpaces(ticketTotal));
            $('#finalTotal').html(numberWithSpaces(feeTotal + ticketTotal));
        });

    $(document).on('input',
        '.ticketFee',
        function(e) {
            var segTotal = 0;
            var ticketTotal = 0;
            var feeTotal = 0;
            $("#receiptItemsTable tbody").children().each(function () {
                const firstDiv = $(this).find("input");
                if (firstDiv.length) {
                    segTotal += Number(firstDiv[1].value);
                    ticketTotal += Number(firstDiv[6].value.replace(/[^0-9.-]+/g, ""));
                    feeTotal += Number(firstDiv[1].value) * Number(firstDiv[7].value.replace(/[^0-9.-]+/g, ""));
                }
            });

            $('#segTotal').html(numberWithSpaces(segTotal));
            $('#feeTotal').html(numberWithSpaces(feeTotal));
            $('#ticketTotal').html(numberWithSpaces(ticketTotal));
            $('#finalTotal').html(numberWithSpaces(feeTotal + ticketTotal));
        });

	$(document).on('click',
        '.removeTicketBtn',
        function(e) {
            e.preventDefault();
            if ($("#dataTable").length) {
                $('#dataTable').dataTable().fnAddData([
                    `<input type="hidden" value="${$($(this).siblings()[0]).val()}" />
                    ${$(this).siblings()[1].outerHTML}<br />
                    ${$(this).siblings()[3].outerHTML}
                    <a href="/" class="btn btn-success btn-sm addTicketBtn">Добавить</a>`,
                    $($(this).parent().siblings()[0]).html(),
                    `<b>${$($($(this).parent().siblings()[1]).find('input')[1]).val()}</b>`,
                    `<b>${$($(this).parent().siblings()[2]).find('input').val()}</b>`,
                    `<b>${$($(this).parent().siblings()[3]).find('input').val()}</b>`
                    ]);

                var my_array = $('#dataTable').dataTable().fnGetNodes();
                var last_element = my_array[my_array.length - 1];      
                $(last_element).insertBefore($('#dataTable tbody tr:first-child'));
            }

            $('#receiptItemsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            if ($("#receiptItemsTable").dataTable().fnSettings().aoData.length) {
                var segTotal = 0;
                var ticketTotal = 0;
                var feeRate = Number($('#feeRate').val().replace(',', '.'));
                $("#receiptItemsTable tbody").children().each(function () {
                    const firstDiv = $(this).find("input");
                    if (firstDiv.length) {
                        segTotal += Number(firstDiv[1].value);
                        ticketTotal += Number(firstDiv[4].value.replace(/[^0-9.-]+/g, ""));
                    }
                });

                $('#segTotal').html(numberWithSpaces(segTotal));
                $('#feeTotal').html(numberWithSpaces(segTotal * feeRate));
                $('#ticketTotal').html(numberWithSpaces(ticketTotal));
                $('#finalTotal').html(numberWithSpaces(segTotal * feeRate + ticketTotal));
            }
        });

    $(document).on('click',
        '.addTicketBtn',
        function (e) {
            e.preventDefault();
            
            $('#receiptItemsTable').dataTable().fnAddData([
                `<input type="hidden" value="${$($(this).siblings()[0]).val()}" />
                ${$(this).siblings()[1].outerHTML}<br />
                ${$(this).siblings()[3].outerHTML}
                <a href="/" class="btn btn-danger btn-sm removeTicketBtn"><i class="glyphicon glyphicon-remove"></i></a>`,
                $($(this).parent().siblings()[0]).html(),
                `<input type="hidden" value="${$(this).parent().siblings()[1].children[1].value}" />
                <input class="ticketRoute" type="text" value="${$(this).parent().siblings()[1].children[0].innerHTML}" />`,
                `<input class="ticketPassenger" type="text" value="${$(this).parent().siblings()[2].children[0].innerHTML}" />`,
                `<input class="ticketPayment" type="text" value="${$($(this).parent().siblings()[3]).html().replace(/ /g, '')
                .replace('</b>', '').replace('<b>', '')}" />`
            ]);

            $('#dataTable').DataTable().row($(this).parents('tr')[0]).remove().draw().show().draw(false);
            //$('#dataTable').dataTable().fnDeleteRow($(this).parents('tr')[0]).show().draw(false);

            var segTotal = 0;
            var ticketTotal = 0;
            var feeRate = Number($('#feeRate').val().replace(',', '.'));
            $("#receiptItemsTable tbody").children().each(function () {
                const firstDiv = $(this).find("input");
                if (firstDiv.length) {
                    segTotal += Number(firstDiv[1].value);
                    ticketTotal += Number(firstDiv[4].value.replace(/[^0-9.-]+/g, ""));
                }
            });

            $('#segTotal').html(numberWithSpaces(segTotal));
            $('#feeTotal').html(numberWithSpaces(segTotal * feeRate));
            $('#ticketTotal').html(numberWithSpaces(ticketTotal));
            $('#finalTotal').html(numberWithSpaces(segTotal * feeRate + ticketTotal));
        });

    $(document).on('click',
        '.addTicketBtnCorp',
        function (e) {
            e.preventDefault();
            
            const exportFirstDiv = $(this).parent().parent().find("input");

            const ticketTypeId = exportFirstDiv[2].value;
            const operationTypeId = exportFirstDiv[3].value;
            var feeRate;
            var perSegment = true;
            var isPercent = false;
            $("#corpFeeRatesDiv tbody").children().each(function () {
                const firstDiv = $(this).find("td");
                if( ticketTypeId == firstDiv[0].innerHTML && firstDiv[1].innerHTML == operationTypeId ) {
                    feeRate = Number(firstDiv[2].innerHTML.replace(",", "."));
                    perSegment = firstDiv[3].innerHTML === 'true';
                    isPercent = firstDiv[4].innerHTML === 'true';
                    console.log(firstDiv);
                    return false;
                }
            });

            $('#receiptItemsTable').dataTable().fnAddData([
                `<input type="hidden" value="${$($(this).siblings()[0]).val()}" />
                ${$(this).siblings()[1].outerHTML}<br />
                ${$(this).siblings()[3].outerHTML}
                <a href="/" class="btn btn-danger btn-sm removeTicketBtnCorp"><i class="glyphicon glyphicon-remove"></i></a>`,
                $($(this).parent().siblings()[0]).html(),
                $($(this).parent().siblings()[1]).html(),
                $($(this).parent().siblings()[2]).html(),
                $($(this).parent().siblings()[3]).html(),
                `<input class="ticketFee" type="text" value="${feeRate}" />
                <input hidden value="${perSegment}" />
                <input hidden value="${isPercent}" />`
            ]);

            $('#dataTable').DataTable().row($(this).parents('tr')[0]).remove().draw().show().draw(false);
            //$('#dataTable').dataTable().fnDeleteRow($(this).parents('tr')[0]).show().draw(false);

            var segTotal = 0;
            var ticketTotal = 0;
            var feeTotal = 0;
            $("#receiptItemsTable tbody").children().each(function () {
                const firstDiv = $(this).find("input");
                const firstTd = $(this).find("td");
                if (firstDiv.length) {
                    segTotal += Number(firstDiv[1].value);
                    var ticketPayment = Number(firstTd[4].innerText.replace(/[^0-9.-]+/g, ""));
                    ticketTotal += ticketPayment;
                    feeTotal += isPercent ? ticketPayment * feeRate / 100 : ( perSegment ? Number(firstDiv[1].value) * feeRate : feeRate);
                }
            });

            $('#segTotal').html(numberWithSpaces(segTotal));
            $('#feeTotal').html(numberWithSpaces(feeTotal));
            $('#ticketTotal').html(numberWithSpaces(ticketTotal));
            $('#finalTotal').html(numberWithSpaces(feeTotal + ticketTotal));
        });

    $(document).on('click',
        '.removeTicketBtnCorp',
        function(e) {
            e.preventDefault();

            const firstDiv = $(this).find("input");

            if ($("#dataTable").length) {
                $('#dataTable').dataTable().fnAddData([
                    `<input type="hidden" value="${$($(this).siblings()[0]).val()}" />
                    ${$(this).siblings()[1].outerHTML}<br />
                    ${$(this).siblings()[3].outerHTML}
                    <a href="/" class="btn btn-success btn-sm addTicketBtn">Добавить</a>`,
                    $($(this).parent().siblings()[0]).html(),
                    $($(this).parent().siblings()[1]).html(),
                    $($(this).parent().siblings()[2]).html(),
                    $($(this).parent().siblings()[3]).html()
                    ]);

                var my_array = $('#dataTable').dataTable().fnGetNodes();
                var last_element = my_array[my_array.length - 1];      
                $(last_element).insertBefore($('#dataTable tbody tr:first-child'));
            }

            $('#receiptItemsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            if ($("#receiptItemsTable").dataTable().fnSettings().aoData.length) {
                var segTotal = 0;
                var ticketTotal = 0;
                var feeTotal = 0;
                $("#receiptItemsTable tbody").children().each(function () {
                    if (firstDiv.length) {
                        segTotal += Number(firstDiv[1].value);
                        ticketTotal += Number(firstDiv[6].value.replace(/[^0-9.-]+/g, ""));
                        feeTotal += Number(firstDiv[1].value) * Number(firstDiv[7].value.replace(/[^0-9.-]+/g, ""));
                    }
                });

                $('#segTotal').html(numberWithSpaces(segTotal));
                $('#feeTotal').html(numberWithSpaces(feeTotal));
                $('#ticketTotal').html(numberWithSpaces(ticketTotal));
                $('#finalTotal').html(numberWithSpaces(feeTotal + ticketTotal));
            }
        });

    $(document).on('click',
        '.addFeeItem',
        function (e) {
            e.preventDefault();

            $('#feeItemsTable').dataTable().fnAddData([
                `<input hidden class="itemId" value="0" />
                <input class="itemName" value="" />`,
                `<input class="itemFeeStr" value="" />`,
                `<button class="btn btn-danger btn-sm removeFeeItem"><i class="glyphicon glyphicon-remove"></i></button>`
            ]);
        });

    $(document).on('click',
        '.removeFeeItem',
        function (e) {
            e.preventDefault();

            $('#feeItemsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);
        });

    $(document).on('click',
        '.addReceiptPayment',
        function (e) {
            e.preventDefault();

            $("#warningMsg").hide();

            var paymentAmount = Big($('#paymentAmountDiv').val().replace(/[,]+/g, "."));
            var paymentTotal = Big(0);
            $("#paidReceiptsTable tbody").children().each(function () {
                if($(this).children().length < 2)
                {
                    return false;
                }
                paymentTotal = paymentTotal.plus(Big($(this).children()[2].innerText.replace(/[^0-9.-]+/g, "")));
            });
            var reminder = paymentAmount.minus(paymentTotal);
            var receiptAmount = Big($(this).parent().siblings()[2].innerText.replace(/[^0-9.-]+/g, ""));
            $('#paidReceiptsTable').dataTable().fnAddData([
                $($(this).parent().siblings()[0]).html(),
                $($(this).parent().siblings()[1]).html(),
                (reminder.gt(receiptAmount) ? $(this).parent().siblings()[2].innerText : numberWithSpaces(reminder.toFixed(2))) +
                `<input hidden class="originalReceiptAmount" value="${$(this).parent().siblings()[2].innerText}" />`,
                `<button class="btn btn-danger btn-sm removeReceiptPayment"><i class="glyphicon glyphicon-remove"></i></button>`
            ]);

            $('#receiptsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            var addToPayment = reminder.gt(receiptAmount) ? receiptAmount : reminder;
            paymentTotal = paymentTotal.plus(addToPayment);

            var paymentReminder = paymentAmount.minus(paymentTotal);
            $('#paymentReminder').html(numberWithSpaces(paymentReminder.toFixed(2)));
            $('#paymentTotal').html(numberWithSpaces(paymentTotal.toFixed(2)));
        });

    $(document).on('click',
        '.removeReceiptPayment',
        function(e) {
            e.preventDefault();

            $("#warningMsg").hide();

            if ($("#receiptsTable").length) {
                $('#receiptsTable').dataTable().fnAddData([
                    $(this).parent().siblings()[0].innerHTML,
                    $(this).parent().siblings()[1].innerText,
                    $($(this).parent().siblings()[2]).find(".originalReceiptAmount").val(),
                    `<button class="btn btn-success addReceiptPayment">Привязать платеж</button>`
                    ]);

                var my_array = $('#receiptsTable').dataTable().fnGetNodes();
                var last_element = my_array[my_array.length - 1];      
                $(last_element).insertBefore($('#receiptsTable tbody tr:first-child'));
            }

            $('#paidReceiptsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            var paymentAmount = Big($('#paymentAmountDiv').val().replace(/[,]+/g, "."));
            if ($("#paidReceiptsTable").dataTable().fnSettings().aoData.length) {
                var paymentTotal = Big(0);
                $("#paidReceiptsTable tbody").children().each(function () {
                    paymentTotal = paymentTotal.plus(Big($(this).children()[2].innerText.replace(/[^0-9.-]+/g, "")));
                });
    
                $('#paymentReminder').html(Big(paymentAmount).subtract(paymentTotal).toFixed(2));
                $('#paymentTotal').html(numberWithSpaces(paymentTotal.toFixed(2)));
            }
            else {
                $('#paymentReminder').html(numberWithSpaces(paymentAmount.toFixed(2)));
                $('#paymentTotal').html(numberWithSpaces(Big(0).toFixed(2)));
            }
        });

    // $(document).on('click',
    //     '#resetOrgDataBtn',
    //     function(e) {
    //         e.preventDefault();
    
    //         var corpOrgAjax = function() {
    //             return $.ajax({
    //                 url: "/CorpReceipt/CorporatorOrganizations",
    //                 type: "GET",
    //                 cache: false,
    //                 success: function(result) {
    //                     $("#payeeSelectDiv").html(result);
    //                     $("#selectBank").val('').trigger("chosen:updated");
    //                 },
    //                 error: function(error) {
    //                     $("#payeeSelectDiv").html();
    //                 }
    //             });
    //         };
    
    //         var orgCorpAjax = function() {
    //             return $.ajax({
    //                 url: "/CorpReceipt/OrganizationCorporators",
    //                 type: "GET",
    //                 cache: false,
    //                 success: function(result) {
    //                     $("#payerSelectDiv").html(result);
    //                 },
    //                 error: function(error) {
    //                     $("#payerSelectDiv").html();
    //                 }
    //             });
    //         };
    
    //         $.when(corpOrgAjax(), orgCorpAjax()).done(function() {
    //             initChosen(
    //                 $('#withBanks').val() == 'true',
    //                 $('#withFees').val() == 'true'
    //             );
    //         });
    //     });

    function loadOrgCorpSelect() {
        $('#selectBank').hide();

        $.get("/CorpReceipt/OrganizationSelect", function (data) {
            $('#selectPayee').select2({
                placeholder: "Выбрать организацию",
                width: "100%",
                data: data.results,
                language: "ru"
            });

            $('#selectPayee').val(null).trigger('change');

            editValueId = $("#editReceiptOrgId").val();
            editValueName = $("#editReceiptOrgName").val();
            if (editValueId) {
                $('#selectPayee').val(editValueId).trigger('change');
                loadOrganizationBank(editValueName, true);
            }
        });

        $.get("/CorpReceipt/CorporatorSelect", function (data) {
            $('#selectPayer').select2({
                placeholder: "Выбрать корпоратора",
                width: "100%",
                data: data.results,
                language: "ru"
            });

            $('#selectPayer').val(null).trigger('change');

            var editValue = $("#editReceiptCorpName").val();
            if (editValue) {
                console.log('editValue');
                $('#selectPayer').val(editValue).trigger('change');
            }
        });
    }

        $(document).on('click',
        '#resetOrgDataBtn',
        function(e) {
            e.preventDefault();
    
            $('#selectPayee').empty();
            $('#selectPayer').empty();
            $('#selectBank').empty();
            loadOrgCorpSelect();
        });

    function initPayer(withBanks, withFees) {
            if( !$("#selectPayer").length )
            {
                return;
            }

            // Options for the observer (which mutations to observe)
            var config = { attributes: true, childList: true, subtree: true };

            // Select the node that will be observed for mutations
            var payerTargetNode = $("#selectPayer").next().find("a.chosen-single span")[0];
                
            // Callback function to execute when mutations are observed
            var payerCallback = function (mutationsList, observer) {
                if(withFees)
                {
                    $.ajax({
                        url: "/CorpClient/CorpFeeList",
                        type: "GET",
                        cache: false,
                        data: {
                            PayerName: $("#selectPayer").next().find("a.chosen-single span").first().html(),
                            PayeeName: $("#selectPayee").next().find("a.chosen-single span").first().html()
                        },
                        success: function (result) {
                            $("#corpFeeRatesDiv").html(result.message);
                        },
                        error: function (error) {
                            console.log('err4');
                        }
                    });
                }

                const mutation = mutationsList[0];

                if ($("#selectPayee").next().find("a.chosen-single span").first().html() === "Выбрать организацию") {
                    $.ajax({
                        url: "/CorpReceipt/CorporatorOrganizations",
                        type: "GET",
                        cache: false,
                        data: { corpName: mutation.target.textContent },
                        success: function (result) {
                            $("#payeeSelectDiv").html(result);
                            $('[data-rel="chosen"],[rel="chosen"]').chosen({ width: "100%", search_contains: true });

                            initPayee(withBanks, withFees);
                        },
                        error: function (error) {
                            $("#payeeSelectDiv").html();
                            console.log('err5');
                        }
                    });
                }
            };

            // Create an observer instance linked to the callback function
            var payerObserver = new MutationObserver(payerCallback);

            // Start observing the target node for configured mutations
            payerObserver.observe(payerTargetNode, config);
        }

    function initPayee(withBanks, withFees) {
        if( !$("#selectPayee").length )
        {
            return;
        }
        // Select the node that will be observed for mutations
        var payeeTargetNode = $("#selectPayee").next().find("a.chosen-single span")[0];
    
        // Options for the observer (which mutations to observe)
        var config = { attributes: true, childList: true, subtree: true };

        // Callback function to execute when mutations are observed
        var payeeCallback = function (mutationsList, observer) {
            const mutation = mutationsList[0];
            if(withBanks)
            {
                $.ajax({
                    url: "/Data/OrganizationFinancialAccounts",
                    type: "GET",
                    cache: false,
                    data: { orgName: mutation.target.textContent },
                    success: function (result) {
                        $("#payeeOrgFinancialAccountsDiv").html(result);
                        $('[data-rel="chosen"],[rel="chosen"]').chosen({ width: "100%", search_contains: true });

                        var editValue = $("#editReceiptBankName").val();
                        if (editValue) {
                            $('#selectBank').val(editValue).trigger('chosen:updated');
                        }
                    },
                    error: function (error) {
                        $("#payeeOrgFinancialAccountsDiv").html();
                    }
                });
            }

            if(withFees)
            {
                $.ajax({
                    url: "/CorpClient/CorpFeeList",
                    type: "GET",
                    cache: false,
                    data: {
                        PayerName: $("#selectPayer").next().find("a.chosen-single span").first().html(),
                        PayeeName: $("#selectPayee").next().find("a.chosen-single span").first().html()
                    },
                    success: function (result) {
                        $("#corpFeeRatesDiv").html(result.message);
                    },
                    error: function (error) {
                        console.log('err2');
                    }
                });
            }

            if ($("#selectPayer").next().find("a.chosen-single span").first().html() === "Выбрать корпоратора") {
                $.when($.ajax({
                    url: "/CorpReceipt/OrganizationCorporators",
                    type: "GET",
                    cache: false,
                    data: { orgName: mutation.target.textContent },
                    success: function (result) {
                        $("#payerSelectDiv").html(result);
                        $('[data-rel="chosen"],[rel="chosen"]').chosen({ width: "100%", search_contains: true });

                        initPayer(withBanks, withFees);
                    },
                    error: function (error) {
                        $("#payerSelectDiv").html();
                    }
                })).done(function(){

                });
            }
        };

        // Create an observer instance linked to the callback function
        var payeeObserver = new MutationObserver(payeeCallback);

        // Start observing the target node for configured mutations
        payeeObserver.observe(payeeTargetNode, config);
    }

    function loadOrganizationBank(orgName, withEdit = false) {
        $.get("/CorpReceipt/OrganizationFinancialAccountSelect",
            { orgName: orgName },
            function (data) {
                $('#selectBank').empty();

                $('#selectBank').select2({
                    placeholder: "Выбрать счет",
                    width: "100%",
                    data: data.results,
                    language: "ru"
                });

                $('#selectBank').val(null).trigger('change');

                if (withEdit)
                {
                    var editValue = $("#editReceiptBankName").val();
                    if (editValue) {
                        $('#selectBank').val(editValue).trigger('change');
                    }
                }

                $('#selectBank').show();
            }
        );
    }

    function initChosen(withBanks = true, withFees = true) {
            $('[data-rel="chosen"],[rel="chosen"]').chosen({ width: "100%", search_contains: true });
    
            initPayee(withBanks, withFees);
            initPayer(withBanks, withFees);
        }

        $(document).on('click',
    '.saveHistoryConfirmBtn[data-toggle=confirmation]',
    function (e) {
        e.preventDefault();
        $(this).confirmation({
            rootSelector: '[data-toggle=confirmation]',
            onConfirm: function (e) {
                var button = $(this);

                $.ajax({
                    url: "/Collection/SaveOfficeBalance",
                    type: "POST",
                    cache: false,
                    data: {
                        total: $("#currentOfficeTotal").val(),
                        _5kBill: $("#currentOffice5kBill").val(),
                        _2kBill: $("#currentOffice2kBill").val()
                    },
                    success: function (result) {
                        $("#getHistoryListBtn").trigger("click");
                    },
                    error: function (error) {
                        console.log(error.message);
                    }
                });
            }
        });
        $(this).confirmation('show');
    });

        $(document).on('input',
        '.groupAmount',
        function(e) {
            $("#warningMsg").hide();

            var paymentAmount = Big($('#expenditureAmountDiv').val().replace(/[,]+/g, "."));
            var paymentTotal = Big(0);
            $("#deskGroupsTable tbody").children().each(function () {
                var value = $(this).children().children("input")[1].value.replace(/[^0-9.-]+/g, "").replace(",",".");
                paymentTotal = paymentTotal.plus(Big(value==="" ? 0 : value));
            });

            var reminder = paymentAmount.minus(paymentTotal);

            $('#expenditureReminder').html(numberWithSpaces(reminder));
            $('#expenditureTotal').html(numberWithSpaces(paymentTotal));
        });

    $(document).mouseup(function(e) 
    {
        var container = $("#sumPopup");
        var toggleButton = $('#toggleSumPopupBtn');

        // if the target of the click isn't the container nor a descendant of the container
        if (!(toggleButton.is(e.target) || container.is(e.target)) && 
            container.has(e.target).length === 0 &&
            toggleButton.has(e.target).length === 0) 
        {
            container.slideUp();
        }
    });

    $(document).on('click',
        '#toggleSumPopupBtn',
        function(e) {
            $('#sumPopup').slideToggle();
    });

function initCleave()
{
    $('.input-numeral').toArray().forEach(function(field){
        new Cleave(field, {
            numeral: true,
            numeralDecimalMark: '.',
            delimiter: ' '
        })
     });    
}

function getReceiptListPDF(model)
{
    if(model.items.length == 0)
        return;

    const docDefinition = {
        content: [],

        styles: {
            bigText: {
                fontSize: 12,
            },
            mediumText: {
                fontSize: 10,
            },
            smallText: {
                fontSize: 8
            },
            tableHeader: {
                bold: true,
                fontSize: 10,
                alignment: 'center'
            }
        },

        pageBreakBefore: function(currentNode, followingNodesOnPage, nodesOnNextPage, previousNodesOnPage) {
            if (currentNode.id === 'NoBreak' && currentNode.pageNumbers.length >= 1 && currentNode.pageNumbers[0] !== 1) {
                return true;
            }
            return false;
        }
    };

    var index = 0;
    var maxItems = model.items.length;
    //$('#mWaitingProgress').show();
    //$('#mWaitingProgress')[0].innerHTML = "1";
    model.items.forEach(function(receiptItem) {

        const itemData = [];

        var headerRow = [];

        headerRow.push({ text: 'Наименование', style: 'tableHeader' });
        headerRow.push({ text: 'Цена', style: 'tableHeader' });
        headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
        headerRow.push({ text: 'Ед. Изм.', style: 'tableHeader' });
        headerRow.push({ text: 'Сумма', style: 'tableHeader' });

        itemData.push(headerRow);

        var dataRow = [];
        var itemCount = 0;
        receiptItem.items.forEach(function (item) {
            dataRow = [];

            itemCount++;
            dataRow.push({ text: item.ticketLabel, style: 'smallText' });
            dataRow.push({ text: item.amountStr, alignment: 'right' });
            dataRow.push({ text: item.segCount, alignment: 'center' });
            dataRow.push({ text: item.amountLabelStr, alignment: 'center' });                        
            dataRow.push({ text: item.amountStr, alignment: 'right' });

            itemData.push(dataRow);
        });

        receiptItem.luggageItems.forEach(function(item) {
            dataRow = [];

            itemCount++;
            dataRow.push({ text: item.ticketLabel, style: 'smallText' });
            dataRow.push({ text: item.amountStr, alignment: 'right' });
            dataRow.push({ text: item.segCount, alignment: 'center' });
            dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
            dataRow.push({ text: item.amountStr, alignment: 'right' });

            itemData.push(dataRow);
        });

        dataRow = [];

        dataRow.push({ text: 'Итого: ', colSpan: 4, alignment: 'right' });
        dataRow.push({});
        dataRow.push({});
        dataRow.push({});
        dataRow.push({ text: receiptItem.itemTotalStr, alignment: 'right' });

        itemData.push(dataRow);

        const feeData = [];

        headerRow = [];

        headerRow.push({ text: '№', style: 'tableHeader' });
        headerRow.push({ text: 'Наименование', style: 'tableHeader' });
        headerRow.push({ text: 'Кол-во', style: 'tableHeader' });
        headerRow.push({ text: 'Ед.', style: 'tableHeader' });
        headerRow.push({ text: 'Цена', style: 'tableHeader' });
        headerRow.push({ text: 'Сумма', style: 'tableHeader' });

        feeData.push(headerRow);

        itemCount = 0;
        receiptItem.taxes.forEach(function(item) {
            dataRow = [];

            itemCount++;
            dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
            dataRow.push({ text: item.ticketLabel, style: 'smallText' });
            dataRow.push({ text: item.segCount, alignment: 'center' });
            dataRow.push({ text: item.amountLabelStr, alignment: 'center' });
            dataRow.push({ text: item.feeStr, alignment: 'right' });
            dataRow.push({ text: item.amountStr, alignment: 'right' });

            feeData.push(dataRow);
        });

        docDefinition.content = docDefinition.content.concat([
            {
                stack: [
                    {
                        text: receiptItem.orgName,
                        style: 'mediumText',
                        bold: true,
                    }
                ],
                id: 'NoBreak'
            },
            {
                text: `Адрес: ${receiptItem.orgAddress}`,
                style: 'mediumText',
                margin: [0, 10, 0, 0]
            },
            {
                text: `АКТ № ${receiptItem.receiptNumber}-П от ${receiptItem.issuedDateTime}`,
                style: 'mediumText',
                bold: true,
                alignment: 'center',
                margin: [0, 25, 0, 0]
            },
            {
                text: `передачи документов`,
                style: 'mediumText',
                bold: true,
                alignment: 'center',
                margin: [0, 10, 0, 0]
            },
            {
                text: `${receiptItem.orgName} передал, а ${receiptItem.payerName} принял следующие документы:`,
                style: 'mediumText',
                margin: [0, 25, 0, 0]
            },
            {
                table: {
                    headerRows: 1,
                    widths: [220, 65, 45, 'auto', '*'],
                    body: itemData
                },
                style: 'mediumText',
                margin: [0, 15, 0, 40]
            },
            {
                stack: [
                    {
                        text: `Итого передано документов на сумму: ${rubles(receiptItem.itemTotal)}. Без НДС`,
                        style: 'mediumText'
                    },
                    {                                        
                        table: {
                            widths: ['*', '*'],
                            heights: [8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8],

                            body: [
                                [`Передал: ${receiptItem.orgName}`,`Принял: ${receiptItem.payerName}`],
                                [`Адрес: ${receiptItem.orgAddress}`,`Адрес: ${receiptItem.payerAddress}`],
                                [`Расчетный счет: ${receiptItem.orgFinancialAccount}`,`Расчетный счет: ${receiptItem.payerFinancialAccount}`],
                                [`Кор. счет: ${receiptItem.orgCorrAccount}`,`Кор. счет: ${receiptItem.payerCorrAccount}`],
                                [`Банк: ${receiptItem.orgBankName}`,`Банк: ${receiptItem.payerBankName}`],
                                [`ИНН: ${receiptItem.orgITN}`,`ИНН: ${receiptItem.payerITN}`],
                                [`КПП: ${receiptItem.orgKPP}`,`КПП: ${receiptItem.payerKPP}`],
                                [`БИК: ${receiptItem.orgBIK}`,`БИК: ${receiptItem.payerBIK}`],
                                [{},{}],
                                [`Сдал _____________________________________________`,`Принял ______________________________________`],
                                [
                                    {
                                        text: `М.П.`, alignment: 'center'
                                    },
                                    {
                                        text: `М.П.`, alignment: 'center'
                                    }
                                ]
                            ]
                        },
                        layout: 'noBorders',
                        style: 'mediumText',
                        margin: [0, 10, 0, 0]
                    }
                ]                
            },
            {
                stack: [
                    {
                        text: `АКТ № ${receiptItem.receiptNumber} от ${receiptItem.issuedDateTime}`,
                        style: 'bigText',
                        bold: true,
                    }
                ],         
                pageBreak: 'before',
            },
            {
                text: `Исполнитель: ${receiptItem.orgName}`,
                style: 'mediumText',
                margin: [0, 35, 0, 0]
            },
            {
                text: `Заказчик: ${receiptItem.payerName}`,
                style: 'mediumText',
                margin: [0, 25, 0, 0]
            },
            {
                table: {
                    headerRows: 1,
                    widths: [20, 220, 45, 'auto', 65, '*'],
                    body: feeData
                },
                style: 'mediumText',
                margin: [0, 35, 0, 0]
            },
            {
                text: `Итого: ${receiptItem.feeTotalStr}`,
                style: 'mediumText',
                alignment: 'right',
                bold: true,
                margin: [0, 35, 0, 0]
            },
            {
                text: `Без налога (НДС): -`,
                style: 'mediumText',
                alignment: 'right',
                bold: true,
                margin: [0, 0, 0, 40]
            },
            {
                stack: [
                    {
                        text: `Всего оказано услуг ${receiptItem.segCountTotal}, на сумму ${receiptItem.feeTotalStr} руб.`,
                        style: 'mediumText'
                    },
                    {
                        text: `${rubles(receiptItem.feeTotal)}`,
                        style: 'mediumText',
                        bold: true
                    },
                    {
                        text: `Вышеперечисленные услуги выполнены полностью и в срок. Заказчик претензий по объему, качеству и срокам оказания услуг не имеет.`,
                        style: 'mediumText',
                        margin: [0, 25, 0, 0]
                    },
                    {
                        columns: [
                            [
                                {
                                    text: 'ИСПОЛНИТЕЛЬ',
                                    bold: true,
                                },
                                {
                                    text: `${receiptItem.orgHeadTitle} ${receiptItem.orgName}`,
                                    margin: [0, 10, 0, 0]
                                },
                                {
                                    text: '________________________________________',
                                    margin: [0, 15, 0, 0]
                                }
                            ],
                            [
                                {
                                    text: 'ЗАКАЗЧИК',
                                    bold: true,
                                },
                                {
                                    text: `${receiptItem.payerName}`,
                                    margin: [0, 10, 0, 0]
                                },
                                {
                                    text: '________________________________________',
                                    margin: [0, 15, 0, 0]
                                }
                            ]
                        ],
                        style: 'mediumText',
                        margin: [0, 25, 0, 0]
                    }
                ]
            }
        ]);

        index++;
        //$('#mWaitingProgress')[0].innerHTML = 100*index/maxItems;
        //alert('123');
    });

    //$('#mWaitingProgress').hide();

    console.log(docDefinition);

    pdfMake.createPdf(docDefinition).open();
}