﻿namespace ZillaIoc
{
    public enum ObjectScope
    {
        /// <summary>
        /// The container will create a new instance every time one is requested.
        /// </summary>
        Transient,
        /// <summary>
        /// The container will create a single instance and return that instance for the duration of the application.
        /// </summary>
        Singleton,
        /// <summary>
        /// The container will create a single instance per HTTP request and return that instance for the duration of the HTTP request.
        /// </summary>
        HttpRequest
    }
}