import { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Droplets, ArrowLeft, Loader2, RefreshCw } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';
import { useLogin, useSendOtp } from '@/hooks/useAuth';

export function OtpPage() {
  const [otp, setOtp] = useState(['', '', '', '', '', '']);
  const [resendCountdown, setResendCountdown] = useState(60);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);
  const { pendingMobile } = useAuthStore();
  const navigate = useNavigate();
  const login = useLogin();
  const sendOtp = useSendOtp();

  useEffect(() => {
    if (!pendingMobile) navigate('/login');
    inputRefs.current[0]?.focus();
  }, [pendingMobile, navigate]);

  useEffect(() => {
    if (resendCountdown <= 0) return;
    const t = setTimeout(() => setResendCountdown(c => c - 1), 1000);
    return () => clearTimeout(t);
  }, [resendCountdown]);

  const handleChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return;
    const newOtp = [...otp];
    newOtp[index] = value.slice(-1);
    setOtp(newOtp);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
    if (newOtp.every(d => d) && newOtp.join('').length === 6) {
      login.mutate({ mobileNumber: pendingMobile!, otpCode: newOtp.join('') });
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-blue-50 dark:from-gray-900 dark:via-gray-900 dark:to-gray-800 flex items-center justify-center p-4">
      <motion.div initial={{ opacity: 0, y: 30 }} animate={{ opacity: 1, y: 0 }} className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-2xl shadow-lg mb-4">
            <Droplets className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Verify OTP</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-1">
            Sent to <strong>{pendingMobile}</strong>
          </p>
        </div>

        <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-xl p-8 border border-gray-100 dark:border-gray-700">
          <div className="flex gap-3 justify-center mb-8">
            {otp.map((digit, i) => (
              <input
                key={i}
                ref={el => { inputRefs.current[i] = el; }}
                type="text"
                inputMode="numeric"
                maxLength={1}
                value={digit}
                onChange={e => handleChange(i, e.target.value)}
                onKeyDown={e => handleKeyDown(i, e)}
                className="w-12 h-12 text-center text-xl font-bold border-2 border-gray-300 dark:border-gray-600 rounded-xl bg-transparent text-gray-900 dark:text-white focus:outline-none focus:border-blue-500 transition-colors"
              />
            ))}
          </div>

          <button
            disabled={otp.some(d => !d) || login.isPending}
            onClick={() => login.mutate({ mobileNumber: pendingMobile!, otpCode: otp.join('') })}
            className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 disabled:opacity-60 text-white font-semibold py-3 px-4 rounded-xl transition-colors mb-4"
          >
            {login.isPending && <Loader2 className="w-4 h-4 animate-spin" />}
            {login.isPending ? 'Verifying...' : 'Verify & Login'}
          </button>

          <div className="text-center">
            {resendCountdown > 0 ? (
              <p className="text-sm text-gray-500">Resend in {resendCountdown}s</p>
            ) : (
              <button
                onClick={() => { sendOtp.mutate(pendingMobile!); setResendCountdown(60); setOtp(['', '', '', '', '', '']); }}
                className="flex items-center gap-1.5 text-sm text-blue-600 hover:underline mx-auto"
              >
                <RefreshCw className="w-3 h-3" /> Resend OTP
              </button>
            )}
          </div>

          <button onClick={() => navigate('/login')} className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 mt-4 mx-auto">
            <ArrowLeft className="w-3 h-3" /> Back to Login
          </button>
        </div>
      </motion.div>
    </div>
  );
}
