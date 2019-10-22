﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Artemis.Core.Events;
using Artemis.Core.Models.Surface;
using Artemis.Core.Services.Interfaces;

namespace Artemis.Core.Services.Storage
{
    public interface ISurfaceService : IArtemisService
    {
        /// <summary>
        ///     Gets or sets the currently active surface configuration
        /// </summary>
        SurfaceConfiguration ActiveSurfaceConfiguration { get; set; }

        /// <summary>
        ///     Gets a read-only list of all surface configurations
        /// </summary>
        ReadOnlyCollection<SurfaceConfiguration> SurfaceConfigurations { get; }

        /// <summary>
        ///     Creates a new surface configuration with the supplied name
        /// </summary>
        /// <param name="name">The name for the new surface configuration</param>
        /// <returns></returns>
        SurfaceConfiguration CreateSurfaceConfiguration(string name);

        /// <summary>
        ///     Deletes the supplied surface configuration, surface configuration may not be the active surface configuration
        /// </summary>
        /// <param name="surfaceConfiguration">The surface configuration to delete, may not be the active surface configuration</param>
        void DeleteSurfaceConfiguration(SurfaceConfiguration surfaceConfiguration);

        /// <summary>
        ///     Saves the provided surface configurations to permanent storage
        /// </summary>
        /// <param name="surfaceConfigurations">The configurations to save</param>
        /// <param name="includeDevices">Whether to also save devices</param>
        void SaveToRepository(List<SurfaceConfiguration> surfaceConfigurations, bool includeDevices);

        /// <summary>
        ///     Saves the provided surface configuration to permanent storage
        /// </summary>
        /// <param name="surfaceConfiguration">The configuration to save</param>
        /// <param name="includeDevices">Whether to also save devices</param>
        void SaveToRepository(SurfaceConfiguration surfaceConfiguration, bool includeDevices);

        /// <summary>
        ///     Occurs when the active device configuration has been changed
        /// </summary>
        event EventHandler<SurfaceConfigurationEventArgs> ActiveSurfaceConfigurationChanged;
    }
}