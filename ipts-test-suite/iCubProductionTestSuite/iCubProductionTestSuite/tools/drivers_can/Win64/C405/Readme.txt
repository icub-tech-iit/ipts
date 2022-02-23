
                   !!!! IMPORTANT NOTICE !!!!!!

In some applications it may become necessary to correlate the current hardware
timestamp of the CAN-PCI/405 with the system time of the host computer. With
this new driver revision the jitter of requesting the hardware timestamp is
reduced drastically. In order to achieve this and to address another issue that
some (real-time) operating systems (e.g RTX) show a non-deterministic timing
during CAN communication because of a performance constraint in the
communication between host computer and the CAN-PCI/405 target hardware
it becomes necessary to update the CAN-PCI/405 firmware and bootloader.
Starting with driver revision 3.7.3 this update is performed implicitly on
the first start of the new driver.

IMPORTANT:     After the bootloader update the driver startup fails and the
               host computer has to be rebooted !! The update of the bootloader
               is irreversible.

PLEASE NOTICE: After the bootloader update using any driver revision prior to
               3.7.3 will fail, because the necessary change couldn't be
               implemented backward compatible !!
               If the use of drivers prior 3.7.3 is necessary, sending the 
               device back to esd is required for a bootloader downgrade.

esd electronic system design gmbh                           January 16, 2007
