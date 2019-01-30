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
                data: { id: $($(this).siblings()[0]).val() },
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
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $($(this).siblings()[0]).val() },
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
                    dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });
                    dataRow.push({ text: item.amountStr, alignment: 'right' });

                    bodyData.push(dataRow);
                });

                dataRow = [];

                itemCount++;
                dataRow.push({ text: (itemCount).toString(), alignment: 'center' });
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
                            text: 'Внимание! Счет действителен для оплаты в течении 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
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
                                    text: `Всего наименований: ${itemCount}, на сумму ${result.totalAmountStr} руб.`,
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
        '.createReceiptPDF2',
        function (e) {
        e.preventDefault();
        $.ajax({
            url: "/CorpReceipt/ReceiptPDFData",
            type: "POST",
            cache: false,
            data: { id: $($(this).siblings()[0]).val() },
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
                            text: 'Внимание! Счет действителен для оплаты в течении 3 банковских дней. Оплата данного счета означает согласие с условиями договора.',
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
                data: { id: $($(this).siblings()[0]).val() },
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

                    dataRow = [];

                    dataRow.push({ text: '1', alignment: 'center' });
                    dataRow.push({ text: 'Сервисный сбор за оформление билета', style: 'smallText' });
                    dataRow.push({ text: result.segCountTotal, alignment: 'center' });
                    dataRow.push({ text: 'полетный\nсегмент', alignment: 'center' });
                    dataRow.push({ text: result.feeRateStr, alignment: 'right' });
                    dataRow.push({ text: result.feeTotalStr, alignment: 'right' });

                    feeData.push(dataRow);

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
                                                    text: `${result.payerHeadTitle} ${result.payerName}`,
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
            e.preventDefault();;
            
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
        '.addReceiptPayment',
        function (e) {
            e.preventDefault();;

            $('#paidReceiptsTable').dataTable().fnAddData([
                $($(this).parent().siblings()[0]).html(),
                $($(this).parent().siblings()[1]).html(),
                `<button class="btn btn-danger btn-sm removeReceiptPayment"><i class="glyphicon glyphicon-remove"></i></button>`
            ]);

            $('#receiptsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            var paymentAmount = Number($('#paymentAmountDiv').val().replace(/[,]+/g, "."));
            var paymentTotal = 0;
            $("#paidReceiptsTable tbody").children().each(function () {
                paymentTotal += Number($(this).children()[1].innerHTML.replace(/[^0-9.-]+/g, ""));
            });

            $('#paymentReminder').html(numberWithSpaces(paymentAmount - paymentTotal));
            $('#paymentTotal').html(numberWithSpaces(paymentTotal));
        });

        $(document).on('click',
        '.removeReceiptPayment',
        function(e) {
            e.preventDefault();
            if ($("#receiptsTable").length) {
                $('#receiptsTable').dataTable().fnAddData([
                    $($(this).parent().siblings()[0]).html(),
                    $($(this).parent().siblings()[1]).html(),
                    `<button class="btn btn-success addReceiptPayment">Привязать платеж</button>`
                    ]);

                var my_array = $('#receiptsTable').dataTable().fnGetNodes();
                var last_element = my_array[my_array.length - 1];      
                $(last_element).insertBefore($('#receiptsTable tbody tr:first-child'));
            }

            $('#paidReceiptsTable').dataTable().fnDeleteRow($(this).parents('tr')[0]);

            var paymentAmount = Number($('#paymentAmountDiv').val().replace(/[,]+/g, "."));
            if ($("#paidReceiptsTable").dataTable().fnSettings().aoData.length) {
                var paymentTotal = 0;
                $("#paidReceiptsTable tbody").children().each(function () {
                    paymentTotal += Number($(this).children()[1].innerHTML.replace(/[^0-9.-]+/g, ""));
                });
    
                $('#paymentReminder').html(numberWithSpaces(paymentAmount - paymentTotal));
                $('#paymentTotal').html(numberWithSpaces(paymentTotal));
            }
            else {
                $('#paymentReminder').html(numberWithSpaces(paymentAmount));
                $('#paymentTotal').html(numberWithSpaces(0));
            }
        });