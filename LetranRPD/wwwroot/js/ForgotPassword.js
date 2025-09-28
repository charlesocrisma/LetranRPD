<script>
    const form = document.getElementById('forgot-password-form');
    const emailInput = document.getElementById('email');
    const resetBtn = document.getElementById('reset-btn');
    const successMessage = document.getElementById('success-message');
    const errorMessage = document.getElementById('error-message');

    form.addEventListener('submit', function(e) {
        e.preventDefault();

    const email = emailInput.value.trim();

    // Hide previous messages
    successMessage.style.display = 'none';
    errorMessage.style.display = 'none';

    // Basic email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!email || !emailRegex.test(email)) {
        errorMessage.style.display = 'block';
    return;
            }

    // Show loading state
    resetBtn.innerHTML = 'Sending...';
    resetBtn.disabled = true;

            // Simulate API call
            setTimeout(() => {
        successMessage.style.display = 'block';
    resetBtn.innerHTML = 'Send Reset Link';
    resetBtn.disabled = false;
    emailInput.value = '';

                // Optionally redirect after a delay
                setTimeout(() => {
        // window.location.href = 'login.html';
    }, 3000);
            }, 2000);
        });

    function goBackToLogin() {
        // Replace with your actual login page URL
        window.location.href = 'login.html';
        }

    // Add some interactive effects
    emailInput.addEventListener('focus', function() {
        this.parentElement.style.transform = 'scale(1.02)';
        });

    emailInput.addEventListener('blur', function() {
        this.parentElement.style.transform = 'scale(1)';
        });
</script>