$(document).ready(function () {
    // Tooltip'leri aktifleþtir
    $('[data-toggle="tooltip"]').tooltip();

    // Form doðrulama mesajlarýnýn görsel stilini iyileþtir
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

    // DataTable eklentisi varsa tablolarý güzelleþtir
    if ($.fn.dataTable) {
        $('.data-table').DataTable({
            "language": {
                "lengthMenu": "Sayfa baþýna _MENU_ kayýt göster",
                "zeroRecords": "Kayýt bulunamadý",
                "info": "_TOTAL_ kayýttan _START_ - _END_ arasý gösteriliyor",
                "infoEmpty": "Kayýt bulunamadý",
                "infoFiltered": "(_MAX_ kayýt arasýndan filtrelendi)",
                "search": "Ara:",
                "paginate": {
                    "first": "Ýlk",
                    "last": "Son",
                    "next": "Sonraki",
                    "previous": "Önceki"
                }
            },
            "responsive": true
        });
    }

    // Silme iþlemi için onay iste
    $('.delete-confirm').on('click', function (e) {
        e.preventDefault();
        var form = $(this).closest('form');

        if (confirm('Bu kaydý silmek istediðinize emin misiniz?')) {
            form.submit();
        }
    });

    // Tarih seçicileri aktifleþtir
    $('.datepicker').datepicker({
        format: 'dd.mm.yyyy',
        language: 'tr',
        autoclose: true,
        todayHighlight: true
    });

    // Bildirim mesajlarýný otomatik kapat
    $('.alert-dismissible').each(function () {
        var alert = $(this);
        setTimeout(function () {
            alert.alert('close');
        }, 5000);
    });

    // Raporlar için grafik oluþturma (Chart.js kullanýlýyorsa)
    if (window.Chart) {
        // Örnek bölüm raporu grafiði
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
                        label: 'Öðrenci Sayýsý',
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

        // Örnek ders raporu grafiði
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
                        label: 'Kayýt Sayýsý',
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

// Kullanýcý iþlemleri için yardýmcý fonksiyonlar
function validatePassword(password) {
    // En az 6 karakter, 1 büyük harf, 1 küçük harf ve 1 rakam kontrolü
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/;
    return regex.test(password);
}

function confirmDelete(message, formId) {
    if (confirm(message || 'Bu kaydý silmek istediðinize emin misiniz?')) {
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