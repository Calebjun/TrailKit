/*
 * Author: Utsah Tamang
 * Description: jQuery for the Customer Management view. Loads and filters customers from
 * /api/customers, expands a row to show charges, and records payments or waivers.
 */
$(function () {
    var waiveTarget = null; // { customerId, chargeId }

    /** Escapes text before it is put into HTML. */
    function esc(text) { return $('<div>').text(text == null ? '' : text).html(); }

    /** Shows an error message above the table. */
    function showError(message) {
        $('#errorBox').text(message).removeClass('d-none');
    }

    /** Builds the Bootstrap badge for a standing. */
    function standingBadge(c) {
        var cls = c.standing === 'Suspended' ? 'bg-danger' : 'bg-success';
        var tip = c.suspensionReason ? ' title="' + esc(c.suspensionReason) + '"' : '';
        return '<span class="badge ' + cls + '"' + tip + '>' + esc(c.standing) + '</span>';
    }

    /** Builds one customer row. */
    function rowHtml(c) {
        return '<tr data-id="' + c.id + '">' +
            '<td>' + esc(c.name) + '</td><td>' + esc(c.email) + '</td>' +
            '<td class="standing">' + standingBadge(c) + '</td>' +
            '<td class="unpaid">$' + c.unpaidTotal.toFixed(2) + '</td>' +
            '<td><button class="btn btn-sm btn-outline-secondary toggle-charges">Charges</button></td></tr>';
    }

    /** Loads customers using the current filters. */
    function loadCustomers() {
        $('#errorBox').addClass('d-none');
        $.getJSON('/api/customers', { standing: $('#standingFilter').val(), search: $('#searchBox').val() })
            .done(function (data) {
                var $body = $('#customerTable tbody').empty();
                data.forEach(function (c) { $body.append(rowHtml(c)); });
                $('#emptyState').toggleClass('d-none', data.length > 0);
            })
            .fail(function () { showError('Could not load customers.'); });
    }

    /** Loads and shows the charges under a customer row. */
    function showCharges($row) {
        var id = $row.data('id');
        $.getJSON('/api/customers/' + id + '/charges').done(function (charges) {
            var html = '<tr class="charges-row"><td colspan="5"><table class="table table-sm mb-0">';
            if (!charges.length) { html += '<tr><td class="text-muted">No charges.</td></tr>'; }
            charges.forEach(function (ch) {
                html += '<tr><td>' + new Date(ch.createdAtUtc).toLocaleDateString() + '</td><td>' + esc(ch.reason) + '</td>' +
                    '<td>$' + ch.amount.toFixed(2) + '</td><td>' + esc(ch.status) +
                    (ch.waivedReason ? ' (' + esc(ch.waivedReason) + ')' : '') + '</td><td>';
                if (ch.status === 'Unpaid') {
                    html += '<button class="btn btn-sm btn-success me-1 pay-btn" data-charge="' + ch.id + '">Record payment</button>' +
                            '<button class="btn btn-sm btn-outline-warning waive-btn" data-charge="' + ch.id + '">Waive</button>';
                }
                html += '</td></tr>';
            });
            html += '</table></td></tr>';
            $row.after(html);
        }).fail(function () { showError('Could not load charges.'); });
    }

    /** Replaces the row's standing and unpaid total and refreshes its charges. */
    function refreshRow($row, customer) {
        $row.find('.standing').html(standingBadge(customer));
        $row.find('.unpaid').text('$' + customer.unpaidTotal.toFixed(2));
        $row.next('.charges-row').remove();
        showCharges($row);
    }

    $('#standingFilter').on('change', loadCustomers);
    var typingTimer;
    $('#searchBox').on('input', function () { clearTimeout(typingTimer); typingTimer = setTimeout(loadCustomers, 300); });

    $('#customerTable').on('click', '.toggle-charges', function () {
        var $row = $(this).closest('tr');
        if ($row.next().hasClass('charges-row')) { $row.next().remove(); } else { showCharges($row); }
    });

    $('#customerTable').on('click', '.pay-btn', function () {
        var $row = $(this).closest('tr.charges-row').prev();
        $.post('/api/customers/' + $row.data('id') + '/charges/' + $(this).data('charge') + '/pay')
            .done(function (customer) { refreshRow($row, customer); })
            .fail(function (xhr) { showError((xhr.responseJSON && xhr.responseJSON.error) || 'Payment failed.'); });
    });

    $('#customerTable').on('click', '.waive-btn', function () {
        var $row = $(this).closest('tr.charges-row').prev();
        waiveTarget = { customerId: $row.data('id'), chargeId: $(this).data('charge'), $row: $row };
        $('#waiveReason').val('');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('waiveModal')).show();
    });

    $('#confirmWaive').on('click', function () {
        var reason = $.trim($('#waiveReason').val());
        if (!reason) { $('#waiveReason').addClass('is-invalid'); return; }
        $('#waiveReason').removeClass('is-invalid');
        $.ajax({
            url: '/api/customers/' + waiveTarget.customerId + '/charges/' + waiveTarget.chargeId + '/waive',
            method: 'POST', contentType: 'application/json', data: JSON.stringify({ reason: reason })
        }).done(function (customer) {
            bootstrap.Modal.getInstance(document.getElementById('waiveModal')).hide();
            refreshRow(waiveTarget.$row, customer);
        }).fail(function (xhr) {
            showError((xhr.responseJSON && xhr.responseJSON.error) || 'Waive failed.');
        });
    });

    loadCustomers();
});
