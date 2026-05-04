document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('preferences-form');

    if (!form) return;

    const progress = document.getElementById('preferences-progress');
    const progressFill = document.getElementById('preferences-progress-fill');
    const daysCurrent = document.getElementById('preferences-days-current');

    const dayInputs = Array.from(form.querySelectorAll('input[name="Days"]'));
    const cityInputs = Array.from(form.querySelectorAll('input[name="City"]'));
    const categoryInputs = Array.from(form.querySelectorAll('input[name="Categories"]'));
    const trackedInputs = [...dayInputs, ...cityInputs, ...categoryInputs];

    const updateState = () => {
        const selectedDay = dayInputs.find((input) => input.checked)?.value ?? '1';
        const hasCity = cityInputs.some((input) => input.checked);
        const hasCategory = categoryInputs.some((input) => input.checked);
        const completedSections = (selectedDay ? 1 : 0) + (hasCity ? 1 : 0) + (hasCategory ? 1 : 0);
        const progressPercent = Math.round((completedSections / 3) * 100);

        if (daysCurrent) {
            daysCurrent.textContent = `${selectedDay} gün seçildi`;
        }

        if (progressFill) {
            progressFill.style.width = `${progressPercent}%`;
        }

        if (progress) {
            progress.setAttribute('aria-valuenow', String(progressPercent));
        }
    };

    trackedInputs.forEach((input) => {
        input.addEventListener('change', updateState);
    });

    updateState();
});
