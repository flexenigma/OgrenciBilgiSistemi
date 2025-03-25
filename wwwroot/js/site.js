$(document).ready(function () {
    // Tooltip'leri aktifle�tir
    $('[data-toggle="tooltip"]').tooltip();

    // Form do�rulama mesajlar�n�n g�rsel stilini iyile�tir
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },
        errorElement: 'div',
        errorClass: 'invalid-feedback',
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    // DataTable eklentisi varsa tablolar� g�zelle�tir
    if ($.fn.dataTable) {
        $('.data-table').DataTable({
            "language": {
                "lengthMenu": "Sayfa ba��na _MENU_ kay�t g�ster",
                "zeroRecords": "Kay�t bulunamad�",
                "info": "_TOTAL_ kay�ttan _START_ - _END_ aras� g�steriliyor",
                "infoEmpty": "Kay�t bulunamad�",
                "infoFiltered": "(_MAX_ kay�t aras�ndan filtrelendi)",
                "search": "Ara:",
                "paginate": {
                    "first": "�lk",
                    "last": "Son",
                    "next": "Sonraki",
                    "previous": "�nceki"
                }
            },
            "responsive": true
        });
    }

    // Silme i�lemi i�in onay iste
    $('.delete-confirm').on('click', function (e) {
        e.preventDefault();
        var form = $(this).closest('form');

        if (confirm('Bu kayd� silmek istedi�inize emin misiniz?')) {
            form.submit();
        }
    });

    // Tarih se�icileri aktifle�tir
    $('.datepicker').datepicker({
        format: 'dd.mm.yyyy',
        language: 'tr',
        autoclose: true,
        todayHighlight: true
    });

    // Bildirim mesajlar�n� otomatik kapat
    $('.alert-dismissible').each(function () {
        var alert = $(this);
        setTimeout(function () {
            alert.alert('close');
        }, 5000);
    });

    // Raporlar i�in grafik olu�turma (Chart.js kullan�l�yorsa)
    if (window.Chart) {
        // �rnek b�l�m raporu grafi�i
        var bolumRaporuCtx = document.getElementById('bolumRaporuChart');
        if (bolumRaporuCtx) {
            var labels = [];
            var data = [];

            $('.bolum-data').each(function () {
                labels.push($(this).data('bolum'));
                data.push($(this).data('sayi'));
            });

            new Chart(bolumRaporuCtx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: '��renci Say�s�',
                        data: data,
                        backgroundColor: 'rgba(33, 133, 208, 0.7)',
                        borderColor: 'rgba(33, 133, 208, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                            precision: 0
                        }
                    }
                }
            });
        }

        // �rnek ders raporu grafi�i
        var dersRaporuCtx = document.getElementById('dersRaporuChart');
        if (dersRaporuCtx) {
            var labels = [];
            var data = [];

            $('.ders-data').each(function () {
                labels.push($(this).data('ders'));
                data.push($(this).data('sayi'));
            });

            new Chart(dersRaporuCtx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Kay�t Say�s�',
                        data: data,
                        backgroundColor: 'rgba(33, 181, 69, 0.7)',
                        borderColor: 'rgba(33, 181, 69, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                            precision: 0
                        }
                    }
                }
            });
        }
    }
});

// Kullan�c� i�lemleri i�in yard�mc� fonksiyonlar
function validatePassword(password) {
    // En az 6 karakter, 1 b�y�k harf, 1 k���k harf ve 1 rakam kontrol�
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/;
    return regex.test(password);
}

function confirmDelete(message, formId) {
    if (confirm(message || 'Bu kayd� silmek istedi�inize emin misiniz?')) {
        document.getElementById(formId).submit();
    }
    return false;
}

function resetForm(formId) {
    document.getElementById(formId).reset();
    return false;
}

function printPage() {
    window.print();
    return false;
}