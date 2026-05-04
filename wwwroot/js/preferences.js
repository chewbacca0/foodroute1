document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('preferences-form');

    if (!form) return;

    const progress = document.getElementById('preferences-progress');
    const progressFill = document.getElementById('preferences-progress-fill');
    const daysCurrent = document.getElementById('preferences-days-current');
    const cityError = document.getElementById('city-client-error');
    const citySection = form.querySelector('.foodroute-preferences__section--2');

    const dayInputs = Array.from(form.querySelectorAll('input[name="Days"]'));
    const cityInputs = Array.from(form.querySelectorAll('input[name="City"]'));
    const categoryInputs = Array.from(form.querySelectorAll('input[name="Categories"]'));
    const trackedInputs = [...dayInputs, ...cityInputs, ...categoryInputs];
    const state = {
        selectedCity: cityInputs.find((input) => input.checked)?.value ?? '',
        errorMessage: ''
    };

    const setCityError = (message) => {
        state.errorMessage = message;

        if (cityError) {
            cityError.textContent = message;
            cityError.hidden = !message;
        }

        if (citySection) {
            citySection.classList.toggle('has-city-error', Boolean(message));
        }
    };

    const updateState = () => {
        const selectedDay = dayInputs.find((input) => input.checked)?.value ?? '1';
        state.selectedCity = cityInputs.find((input) => input.checked)?.value ?? '';
        const hasCity = Boolean(state.selectedCity);
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

        if (hasCity && state.errorMessage) {
            setCityError('');
        }
    };

    trackedInputs.forEach((input) => {
        input.addEventListener('change', updateState);
    });

    form.addEventListener('submit', (event) => {
        state.selectedCity = cityInputs.find((input) => input.checked)?.value ?? '';

        if (!state.selectedCity) {
            event.preventDefault();
            setCityError('Lütfen bir şehir seçiniz.');
            cityError?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    });

    updateState();
});
