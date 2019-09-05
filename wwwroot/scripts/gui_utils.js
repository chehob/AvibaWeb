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
    $div.load($div.data("url"), function() {
        $("#saleButton").click();
    });
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
                            text: `Поставщик: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Покупатель: ${result.payerName}`,
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
                            text: `Поставщик: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Покупатель: ${result.payerName}`,
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
                            text: `Поставщик: ${result.orgName}`,
                            style: 'mediumText',
                            bold: true,
                            margin: [0, 15, 0, 0]
                        },
                        {
                            text: `${result.orgAddress}`,
                            style: 'mediumText',
                        },
                        {
                            text: `Покупатель: ${result.payerName}`,
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
        ".createReportPDF",
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
                                                            text: `Сдал _____________________________________________`,
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
                                                            text: '________________________________________',
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

            $('#dataTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

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

            $('#dataTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

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

        $.get("/CorpReceipt/CounterpartySelect", function (data) {
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
            numeralDecimalMark: ['.', ','],
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