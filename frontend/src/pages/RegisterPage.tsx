import { useState, useRef } from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { motion, AnimatePresence } from 'framer-motion';
import { Droplets, User, Phone, Mail, ArrowRight, Loader2, RefreshCw, CheckCircle2 } from 'lucide-react';
import { useSendOtp, useRegister } from '@/hooks/useAuth';

const step1Schema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  mobileNumber: z.string().regex(/^\+?[1-9]\d{9,14}$/, 'Enter a valid mobile number (e.g. +919876543210)'),
  email: z.string().email('Invalid email').optional().or(z.literal('')),
});
type Step1Data = z.infer<typeof step1Schema>;

export function RegisterPage() {
  const [step, setStep] = useState<1 | 2>(1);
  const [formData, setFormData] = useState<Step1Data | null>(null);
  const [otp, setOtp] = useState(['', '', '', '', '', '']);
  const [resendCountdown, setResendCountdown] = useState(0);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  const sendOtp = useSendOtp({ skipRedirect: true });
  const register = useRegister();

  const { register: reg, handleSubmit, formState: { errors } } = useForm<Step1Data>({
    resolver: zodResolver(step1Schema),
  });

  const handleStep1 = async (data: Step1Data) => {
    await sendOtp.mutateAsync(data.mobileNumber);
    setFormData(data);
    setStep(2);
    setResendCountdown(60);
    startCountdown();
    setTimeout(() => inputRefs.current[0]?.focus(), 100);
  };

  const startCountdown = () => {
    let t = 60;
    const iv = setInterval(() => {
      t--;
      setResendCountdown(t);
      if (t <= 0) clearInterval(iv);
    }, 1000);
  };

  const handleOtpChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return;
    const next = [...otp];
    next[index] = value.slice(-1);
    setOtp(next);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
  };

  const handleOtpKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  const handleResend = async () => {
    if (!formData) return;
    await sendOtp.mutateAsync(formData.mobileNumber);
    setOtp(['', '', '', '', '', '']);
    setResendCountdown(60);
    startCountdown();
    inputRefs.current[0]?.focus();
  };

  const handleCreateAccount = () => {
    if (!formData) return;
    const otpCode = otp.join('');
    if (otpCode.length !== 6) return;
    register.mutate({
      name: formData.name,
      mobileNumber: formData.mobileNumber,
      otpCode,
      email: formData.email || undefined,
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-blue-50 dark:from-gray-900 dark:via-gray-900 dark:to-gray-800 flex items-center justify-center p-4">
      <motion.div initial={{ opacity: 0, y: 30 }} animate={{ opacity: 1, y: 0 }} className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-2xl shadow-lg mb-4">
            <Droplets className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Create Account</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-1">Join WaterLevelRecognizer platform</p>
        </div>

        {/* Step indicator */}
        <div className="flex items-center justify-center gap-3 mb-6">
          <div className={`flex items-center gap-1.5 text-sm font-medium ${step === 1 ? 'text-blue-600' : 'text-green-600'}`}>
            {step === 2 ? <CheckCircle2 className="w-4 h-4" /> : <span className="w-5 h-5 rounded-full bg-blue-600 text-white flex items-center justify-center text-xs">1</span>}
            Your Details
          </div>
          <div className="w-8 h-px bg-gray-300 dark:bg-gray-600" />
          <div className={`flex items-center gap-1.5 text-sm font-medium ${step === 2 ? 'text-blue-600' : 'text-gray-400'}`}>
            <span className={`w-5 h-5 rounded-full flex items-center justify-center text-xs ${step === 2 ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-700 text-gray-500'}`}>2</span>
            Verify OTP
          </div>
        </div>

        <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-xl border border-gray-100 dark:border-gray-700 overflow-hidden">
          <AnimatePresence mode="wait">
            {step === 1 ? (
              <motion.div key="step1" initial={{ opacity: 0, x: -20 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: -20 }} className="p-8">
                <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-5">Enter your details</h2>
                <form onSubmit={handleSubmit(handleStep1)} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">Full Name *</label>
                    <div className="relative">
                      <User className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                      <input {...reg('name')} type="text" placeholder="Enter your full name"
                        className="w-full pl-10 pr-4 py-3 border border-gray-300 dark:border-gray-600 rounded-xl bg-transparent text-gray-900 dark:text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-sm" />
                    </div>
                    {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">Mobile Number *</label>
                    <div className="relative">
                      <Phone className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                      <input {...reg('mobileNumber')} type="tel" placeholder="+91XXXXXXXXXX"
                        className="w-full pl-10 pr-4 py-3 border border-gray-300 dark:border-gray-600 rounded-xl bg-transparent text-gray-900 dark:text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-sm" />
                    </div>
                    {errors.mobileNumber && <p className="text-red-500 text-xs mt-1">{errors.mobileNumber.message}</p>}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">Email <span className="text-gray-400 font-normal">(Optional)</span></label>
                    <div className="relative">
                      <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                      <input {...reg('email')} type="email" placeholder="email@example.com"
                        className="w-full pl-10 pr-4 py-3 border border-gray-300 dark:border-gray-600 rounded-xl bg-transparent text-gray-900 dark:text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-sm" />
                    </div>
                    {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email.message}</p>}
                  </div>

                  <button type="submit" disabled={sendOtp.isPending}
                    className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 disabled:opacity-60 text-white font-semibold py-3 px-4 rounded-xl transition-colors mt-2">
                    {sendOtp.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : <ArrowRight className="w-4 h-4" />}
                    {sendOtp.isPending ? 'Sending OTP...' : 'Send OTP'}
                  </button>
                </form>

                <div className="mt-6 text-center">
                  <p className="text-sm text-gray-500">Already have an account?{' '}
                    <Link to="/login" className="text-blue-600 hover:underline font-medium">Sign In</Link>
                  </p>
                </div>
              </motion.div>
            ) : (
              <motion.div key="step2" initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: 20 }} className="p-8">
                <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-1">Verify your number</h2>
                <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
                  OTP sent to <strong className="text-gray-700 dark:text-gray-200">{formData?.mobileNumber}</strong>
                  <button onClick={() => setStep(1)} className="ml-2 text-blue-600 hover:underline text-xs">Change</button>
                </p>

                {/* OTP boxes */}
                <div className="flex gap-2 justify-center mb-6">
                  {otp.map((digit, i) => (
                    <input key={i}
                      ref={el => { inputRefs.current[i] = el; }}
                      type="text" inputMode="numeric" maxLength={1} value={digit}
                      onChange={e => handleOtpChange(i, e.target.value)}
                      onKeyDown={e => handleOtpKeyDown(i, e)}
                      className="w-11 h-12 text-center text-xl font-bold border-2 border-gray-300 dark:border-gray-600 rounded-xl bg-transparent text-gray-900 dark:text-white focus:outline-none focus:border-blue-500 transition-colors" />
                  ))}
                </div>

                <button
                  onClick={handleCreateAccount}
                  disabled={otp.some(d => !d) || register.isPending}
                  className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 disabled:opacity-60 text-white font-semibold py-3 px-4 rounded-xl transition-colors mb-4">
                  {register.isPending && <Loader2 className="w-4 h-4 animate-spin" />}
                  {register.isPending ? 'Creating account...' : 'Create Account'}
                </button>

                <div className="text-center">
                  {resendCountdown > 0 ? (
                    <p className="text-sm text-gray-500">Resend OTP in {resendCountdown}s</p>
                  ) : (
                    <button onClick={handleResend} disabled={sendOtp.isPending}
                      className="flex items-center gap-1.5 text-sm text-blue-600 hover:underline mx-auto disabled:opacity-50">
                      <RefreshCw className="w-3 h-3" /> Resend OTP
                    </button>
                  )}
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </motion.div>
    </div>
  );
}
