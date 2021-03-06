﻿// ==================================================================================================================                                                                                          
//        ,::i                                                           BBB                
//       BBBBBi                                                         EBBB                
//      MBBNBBU                                                         BBB,                
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2010-2011 apworks.codeplex.com.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using System;
using System.Linq;
using System.Reflection;
using Apworks.Properties;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the domain event handler that is defined within the scope of
    /// an aggregate root and handles the domain event when <c>SourcedAggregateRoot.RaiseEvent&lt;TEvent&gt;</c>
    /// is called.
    /// </summary>
    public sealed class InlineDomainEventHandler : IDomainEventHandler
    {
        #region Private Fields
        private readonly Type domainEventType;
        private readonly Func<IDomainEvent, bool> action;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>InlineDomainEventHandler</c> class.
        /// </summary>
        /// <param name="aggregateRoot">The instance of the aggregate root within which the domain event
        /// was raised and handled.</param>
        /// <param name="mi">The method which handles the domain event.</param>
        public InlineDomainEventHandler(ISourcedAggregateRoot aggregateRoot, MethodInfo mi)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters == null || parameters.Count() == 0)
            {
                throw new ArgumentException(string.Format(Resources.EX_INVALID_HANDLER_SIGNATURE, mi.Name), "mi");
            }
            domainEventType = parameters[0].ParameterType;
            action = domainEvent =>
            {
                try
                {
                    mi.Invoke(aggregateRoot, new object[] { domainEvent });
                    return true;
                }
                catch { return false; }
            };
        }
        #endregion

        #region IDomainEventHandler Members
        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="message">The domain event to be handled.</param>
        public bool Handle(IDomainEvent message)
        {
            return action(message);
        }
        /// <summary>
        /// Checks whether the specified domain event could be handled by the current handler.
        /// </summary>
        /// <param name="domainEvent">The domain event to be checked.</param>
        /// <returns>True if the specified domain event can be handled by the current handler, otherwise false.</returns>
        public bool CanHandle(IDomainEvent domainEvent)
        {
            return domainEventType.Equals(domainEvent.GetType());
        }

        #endregion
    }
}
