import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../css/register.css';

const API_URL = 'https://www.newtlike.com:3000';

const Register = () => {
  const [formData, setFormData] = useState({ 
    username: '', 
    password: '', 
    confirmPassword: '' 
  });
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const navigate = useNavigate();
  const { user } = useAuth();

  // Redirect if already logged in
  useEffect(() => {
    if (user) {
      navigate('/');
    }
  }, [user, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear error for this field when user types
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    // Username validation
    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 4) {
      newErrors.username = 'Username must be at least 4 characters';
    } else if (!/^[a-zA-Z0-9_]+$/.test(formData.username)) {
      newErrors.username = 'Username can only contain letters, numbers, and underscores';
    }

    // Password validation
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    } else if ((formData.password.match(/[a-zA-Z]/g) || []).length < 3) {
      newErrors.password = 'Password must contain at least 3 letters';
    } else if ((formData.password.match(/[0-9]/g) || []).length < 2) {
      newErrors.password = 'Password must contain at least 2 numbers';
    } else if (formData.username && formData.password.toLowerCase().includes(formData.username.toLowerCase())) {
      newErrors.password = 'Password cannot contain your username';
    }

    // Confirm password validation
    if (!formData.confirmPassword) {
      newErrors.confirmPassword = 'Please confirm your password';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setApiError('');
    setSuccessMessage('');

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      console.log('Attempting registration for:', formData.username);
      console.log('API URL:', `${API_URL}/api/users/create`);
      // hvis det ikke fungere sådan her skal der stå ${API_URL}/api/users/login i fetch
      const response = await fetch(`/api/users/create`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: formData.username,
          password: formData.password
        }),
        credentials: 'include'
      });

      console.log('Response status:', response.status);
      const data = await response.json();
      console.log('Response data:', data);

      if (!response.ok) {
        throw new Error(data.error || data.Error || data.message || 'Registration failed');
      }

      // Show success message
      setSuccessMessage('Registration successful! Redirecting to login...');
      
      // Clear form
      setFormData({ username: '', password: '', confirmPassword: '' });
      
      // Redirect to login page after 2 seconds
      setTimeout(() => {
        navigate('/login');
      }, 2000);
      
    } catch (error) {
      console.error('Registration error:', error);
      setApiError(error.message || 'An error occurred during registration. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="register-container">
      <h1>Register</h1>
      
      {successMessage && (
        <div className="success-message">
          {successMessage}
        </div>
      )}

      {apiError && (
        <div className="error-message">
          {apiError}
        </div>
      )}

      <form onSubmit={handleRegister} className="register-form">
        <div className="form-group">
          <label htmlFor="username">
            Username:
          </label>
          <input
            type="text"
            id="username"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
            disabled={isLoading}
            className={errors.username ? 'error' : ''}
            autoComplete="username"
          />
          {errors.username && (
            <span className="field-error">{errors.username}</span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="password">
            Password:
          </label>
          <input
            type={showPassword ? "text" : "password"}
            id="password"
            name="password"
            value={formData.password}
            onChange={handleInputChange}
            disabled={isLoading}
            className={errors.password ? 'error' : ''}
            autoComplete="new-password"
          />
          {errors.password && (
            <span className="field-error">{errors.password}</span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="confirmPassword">
            Confirm Password:
          </label>
          <input
            type={showPassword ? "text" : "password"}
            id="confirmPassword"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleInputChange}
            disabled={isLoading}
            className={errors.confirmPassword ? 'error' : ''}
            autoComplete="new-password"
          />
          {errors.confirmPassword && (
            <span className="field-error">{errors.confirmPassword}</span>
          )}
        </div>

        <div className="form-group">
          <label style={{ flexDirection: 'row', alignItems: 'center', gap: '8px' }}>
            <input
              type="checkbox"
              checked={showPassword}
              onChange={(e) => setShowPassword(e.target.checked)}
              disabled={isLoading}
            />
            Show passwords
          </label>
        </div>

        <button 
          type="submit" 
          disabled={isLoading}
          className="register-button"
        >
          {isLoading ? 'Registering...' : 'Register'}
        </button>
      </form>

      <div className="login-link">
        <p>
          Already have an account? <Link to="/login">Login here</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;