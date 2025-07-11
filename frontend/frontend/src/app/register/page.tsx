"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import ReCAPTCHA from "react-google-recaptcha";

export default function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [recaptchaToken, setRecaptchaToken] = useState<string | null>(null);
  const router = useRouter();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");
    if (!username.trim()) {
      setError("Username is required.");
      return;
    }
    if (password.length < 6) {
      setError("Password must be at least 6 characters long.");
      return;
    }
    if (!/[0-9]/.test(password)) {
      setError("Password must have at least one digit (0-9).");
      return;
    }
    if (!/[A-Z]/.test(password)) {
      setError("Password must have at least one uppercase letter (A-Z).");
      return;
    }
    if (!recaptchaToken) {
      setError("Please complete the CAPTCHA.");
      return;
    }
    setLoading(true);
    try {
      const res = await fetch("http://localhost:5086/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password, recaptchaToken }),
      });
      if (!res.ok) {
        const msg = await res.text();
        try {
          const json = JSON.parse(msg);
          if (Array.isArray(json)) {
            const duplicate = json.find((e: { code?: string }) => e.code === "DuplicateUserName");
            if (duplicate) {
              setError("This username is already taken.");
              setLoading(false);
              return;
            }
            if (json[0]?.description) {
              setError(json[0].description);
              setLoading(false);
              return;
            }
          }
        } catch {
          setError(msg || "Registration failed");
          setLoading(false);
          return;
        }
        setError("Registration failed");
        setLoading(false);
        return;
      }
      router.push("/"); 
    } catch (err: unknown) {
      if (err instanceof Error) {
        setError(err.message || "Registration failed");
      } else {
        setError("Registration failed");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-black via-purple-900 to-purple-700 p-4 relative">
      <div className="absolute top-4 left-4 text-2xl font-extrabold tracking-wide text-purple-300 select-none drop-shadow">
        SPOTIBUDS
      </div>
      <form
        onSubmit={handleSubmit}
        className="flex flex-col gap-4 w-full max-w-xs sm:max-w-sm bg-black/60 p-6 sm:p-8 rounded-xl shadow-lg border border-purple-800"
      >
        <h1 className="text-2xl font-bold mb-4 text-center text-purple-200">Register</h1>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUsername(e.target.value)}
          className="border border-purple-700 bg-black/40 text-purple-100 placeholder:text-purple-400 p-2 rounded focus:outline-none focus:ring-2 focus:ring-purple-600"
          required
        />
        <input
          type="email"
          placeholder="Email"
          className="border border-purple-700 bg-black/40 text-purple-100 placeholder:text-purple-400 p-2 rounded focus:outline-none focus:ring-2 focus:ring-purple-600"
          required
        />
        <div className="relative">
          <input
            type={showPassword ? "text" : "password"}
            placeholder="Password"
            value={password}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)}
            className="border border-purple-700 bg-black/40 text-purple-100 placeholder:text-purple-400 p-2 rounded w-full focus:outline-none focus:ring-2 focus:ring-purple-600 pr-10"
            required
          />
          <button
            type="button"
            aria-label={showPassword ? "Hide password" : "Show password"}
            onClick={() => setShowPassword((v: boolean) => !v)}
            className="absolute right-2 top-1/2 -translate-y-1/2 p-1 text-purple-400 hover:text-purple-200 focus:outline-none"
            tabIndex={0}
          >
            {showPassword ? (
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 12s3.75-7.5 9.75-7.5 9.75 7.5 9.75 7.5-3.75 7.5-9.75 7.5S2.25 12 2.25 12z" />
                <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M3.98 8.223A10.477 10.477 0 002.25 12s3.75 7.5 9.75 7.5c1.956 0 3.74-.5 5.272-1.277M21.75 12c-.512-1.276-1.318-2.735-2.418-4.023M9.53 9.53a3 3 0 014.24 4.24m-6.01-6.01A7.477 7.477 0 0112 6.75c4.243 0 7.75 3.75 7.75 3.75s-.457.684-1.23 1.57M9.53 9.53L3.98 8.223m0 0l1.768 1.768m0 0l1.768 1.768m0 0l1.768 1.768m0 0l1.768 1.768m0 0l1.768 1.768" />
              </svg>
            )}
          </button>
        </div>
        <div className="flex justify-center my-2">
          <ReCAPTCHA
            sitekey="6Lc_XnUrAAAAAJifrj3oH0a2EGx6ml4pWIrlYKus"
            theme="dark"
            onChange={(token: string | null) => setRecaptchaToken(token)}
          />
        </div>
        <button
          type="submit"
          className="bg-gradient-to-r from-purple-700 to-purple-500 text-white py-2 rounded font-semibold hover:from-purple-800 hover:to-purple-600 transition border border-purple-900 shadow"
          disabled={loading}
        >
          {loading ? "Registering..." : "Register"}
        </button>
        {error && <div className="text-red-400 text-sm text-center">{error}</div>}
        <div className="flex items-center justify-center mt-4 gap-2">
          <span className="text-purple-200">Already have an account?</span>
          <Link
            href="/"
            className="text-purple-300 font-semibold hover:underline hover:text-purple-100 transition"
          >
            Login
          </Link>
        </div>
      </form>
    </div>
  );
}
