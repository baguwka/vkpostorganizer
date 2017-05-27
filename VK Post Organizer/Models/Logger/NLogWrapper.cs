using System;
using NLog;

namespace vk.Models.Logger {
    public class NLogWrapper : ILogger {
        private readonly NLog.ILogger _logger;

        public NLogWrapper() {
            _logger = new NullLogger(null);
        }

        public void Debug<T>(T value) {
            _logger.Debug(value);
        }

        public void Debug(object value) {
            _logger.Debug(value);
        }

        public void Debug(Exception exception, string message) {
            _logger.Debug(exception, message);
        }

        public void Debug(string message) {
            _logger.Debug(message);
        }

        public void Info<T>(T value) {
            _logger.Info(value);
        }

        public void Info(object value) {
            _logger.Info(value);
        }

        public void Info(Exception exception, string message) {
            _logger.Info(exception, message);
        }

        public void Info(string message) {
            _logger.Info(message);
        }

        public void Warn<T>(T value) {
            _logger.Warn(value);
        }

        public void Warn(object value) {
            _logger.Warn(value);
        }

        public void Warn(Exception exception, string message) {
            _logger.Warn(exception, message);
        }

        public void Warn(string message) {
            _logger.Warn(message);
        }

        public void Error<T>(T value) {
            _logger.Error(value);
        }

        public void Error(object value) {
            _logger.Error(value);
        }

        public void Error(Exception exception, string message) {
            _logger.Error(exception, message);
        }

        public void Error(string message) {
            _logger.Error(message);
        }

        public void Fatal<T>(T value) {
            _logger.Fatal(value);
        }

        public void Fatal(object value) {
            _logger.Fatal(value);
        }

        public void Fatal(Exception exception, string message) {
            _logger.Fatal(exception, message);
        }

        public void Fatal(string message) {
            _logger.Fatal(message);
        }

        public void Trace<T>(T value) {
            _logger.Trace(value);
        }

        public void Trace(object value) {
            _logger.Trace(value);
        }

        public void Trace(Exception exception, string message) {
            _logger.Trace(exception, message);
        }

        public void Trace(string message) {
            _logger.Trace(message);
        }
    }
}