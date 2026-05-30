import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authApi } from '@/api/auth.api';
import { useAuthStore } from '@/store/authStore';
import toast from 'react-hot-toast';

export function useSendOtp(options?: { skipRedirect?: boolean }) {
  const { setPendingMobile } = useAuthStore();
  const navigate = useNavigate();
  return useMutation({
    mutationFn: (mobileNumber: string) => authApi.sendOtp(mobileNumber),
    onSuccess: (_, mobileNumber) => {
      setPendingMobile(mobileNumber);
      toast.success('OTP sent! Check backend logs for the code.');
      if (!options?.skipRedirect) navigate('/otp');
    },
  });
}

export function useLogin() {
  const { setAuth } = useAuthStore();
  const navigate = useNavigate();
  return useMutation({
    mutationFn: (data: { mobileNumber: string; otpCode: string }) => authApi.login(data),
    onSuccess: (response) => {
      const data = response.data.data!;
      setAuth(data);
      toast.success(`Welcome back, ${data.user.name}!`);
      navigate('/dashboard');
    },
  });
}

export function useRegister() {
  const { setAuth } = useAuthStore();
  const navigate = useNavigate();
  return useMutation({
    mutationFn: (data: { name: string; mobileNumber: string; otpCode: string; email?: string }) =>
      authApi.register(data),
    onSuccess: (response) => {
      const data = response.data.data!;
      setAuth(data);
      toast.success('Registration successful!');
      navigate('/dashboard');
    },
  });
}

export function useLogout() {
  const { logout } = useAuthStore();
  const navigate = useNavigate();
  return () => {
    logout();
    navigate('/login');
    toast.success('Logged out successfully.');
  };
}
