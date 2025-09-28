// ForgotPassword.js
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('forgot-password-form');
    const emailInput = document.getElementById('email');
    const resetBtn = document.getElementById('reset-btn');
    const successMessage = document.getElementById('success-message');
    const errorMessage = document.getElementById('error-message');

    // Handle form submission
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const email = emailInput.value.trim();

        // Hide previous messages
        hideAllMessages();

        // Basic email validation
        if (!isValidEmail(email)) {
            showMessage('Please enter a valid email address.', 'error');
            return;
        }

        // Show loading state
        setLoadingState(true);

        try {
            // Create FormData to send the form data
            const formData = new FormData();
            formData.append('email', email);

            // Add anti-forgery token
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            formData.append('__RequestVerificationToken', token);

            // Send the request to the server
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    showMessage(result.message || 'Password reset link has been sent to your email address. Please check your inbox.', 'success');
                    emailInput.value = ''; // Clear the input

                    // Optional: Redirect to login after 5 seconds
                    setTimeout(() => {
                        window.location.href = '/Account/Login';
                    }, 5000);
                } else {
                    showMessage(result.message || 'An error occurred. Please try again.', 'error');
                }
            } else {
                // Handle non-JSON responses or server errors
                if (response.headers.get('content-type')?.includes('application/json')) {
                    const result = await response.json();
                    showMessage(result.message || 'An error occurred. Please try again.', 'error');
                } else {
                    showMessage('An error occurred. Please try again later.', 'error');
                }
            }
        } catch (error) {
            console.error('Error:', error);
            showMessage('Network error. Please check your connection and try again.', 'error');
        } finally {
            setLoadingState(false);
        }
    });

    // Email input validation on blur
    emailInput.addEventListener('blur', function () {
        const email = this.value.trim();
        if (email && !isValidEmail(email)) {
            showMessage('Please enter a valid email address.', 'error');
        } else {
            hideAllMessages();
        }
    });

    // Clear messages when user starts typing
    emailInput.addEventListener('input', function () {
        hideAllMessages();
    });

    // Utility functions
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    function showMessage(message, type) {
        hideAllMessages();

        if (type === 'success') {
            successMessage.textContent = message;
            successMessage.style.display = 'block';
            successMessage.style.animation = 'fadeIn 0.3s ease';
        } else if (type === 'error') {
            errorMessage.textContent = message;
            errorMessage.style.display = 'block';
            errorMessage.style.animation = 'fadeIn 0.3s ease';
        }
    }

    function hideAllMessages() {
        successMessage.style.display = 'none';
        errorMessage.style.display = 'none';
    }

    function setLoadingState(loading) {
        if (loading) {
            resetBtn.disabled = true;
            resetBtn.textContent = 'Sending...';
            resetBtn.style.opacity = '0.7';
            resetBtn.style.cursor = 'not-allowed';
        } else {
            resetBtn.disabled = false;
            resetBtn.textContent = 'Send Reset Link';
            resetBtn.style.opacity = '1';
            resetBtn.style.cursor = 'pointer';
        }
    }

    // Add some visual feedback for the input field
    emailInput.addEventListener('focus', function () {
        this.style.transform = 'scale(1.02)';
        this.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.1)';
    });

    emailInput.addEventListener('blur', function () {
        this.style.transform = 'scale(1)';
        this.style.boxShadow = 'none';
    });
});

// CSS animations (add this to your CSS file if not already present)
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(-10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .success-message, .error-message {
        animation: fadeIn 0.3s ease;
        margin-bottom: 20px;
        padding: 15px;
        border-radius: 10px;
        text-align: center;
        font-size: 14px;
        font-weight: 500;
    }

    .success-message {
        background: linear-gradient(135deg, #4CAF50, #45a049);
        color: white;
        border: 1px solid #4CAF50;
    }

    .error-message {
        background: linear-gradient(135deg, #f44336, #d32f2f);
        color: white;
        border: 1px solid #f44336;
    }
`;
document.head.appendChild(style);